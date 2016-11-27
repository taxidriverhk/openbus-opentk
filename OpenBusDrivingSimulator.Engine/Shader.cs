using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Engine
{
    internal enum ShaderAttribute
    {
        POSITION = 0,
        NORMAL = 1,
        TEXCOORD = 2
    }

    internal enum ShaderUniform
    {
        PROJECTION_MATRIX = 0,
        VIEW_MATRIX = 1,
        MODEL_MATRIX = 2
    }

    internal class ShaderProgram
    {
        private abstract class ShaderVariable
        {
            public string Name;
            public int Location;
            public int Size;
        }

        private class AttributeVariable : ShaderVariable
        {
            public ActiveAttribType Type;
        }

        private class UniformVariable : ShaderVariable
        {
            public ActiveUniformType Type;
        }

        private const int MAX_VAR_LENGTH = 256;
        private static readonly Dictionary<ShaderAttribute, string> attributeVarNameMap = new Dictionary<ShaderAttribute, string>()
        {
            { ShaderAttribute.POSITION, "vPosition" },
            { ShaderAttribute.NORMAL, "vNormal" },
            { ShaderAttribute.TEXCOORD, "vTexCoord" }
        };

        private int programId;
        private bool loadSuccess;

        private Dictionary<string, AttributeVariable> attributes;
        private Dictionary<string, UniformVariable> uniforms;

        internal ShaderProgram()
        {
            programId = 0;
            loadSuccess = false;

            attributes = new Dictionary<string, AttributeVariable>();
            uniforms = new Dictionary<string, UniformVariable>();
        }

        internal void BindVertexAttribPointer(ShaderAttribute attributeType)
        {
            string varName = attributeVarNameMap[attributeType];
            if (attributes.ContainsKey(varName))
            {
                int offset = 0,
                    size = 3;
                switch (attributeType)
                {
                    case ShaderAttribute.POSITION:
                        offset = 0;
                        size = 3;
                        break;
                    case ShaderAttribute.NORMAL:
                        offset = Vector3.SizeInBytes;
                        size = 3;
                        break;
                    case ShaderAttribute.TEXCOORD:
                        offset = 2 * Vector3.SizeInBytes;
                        size = 2;
                        break;
                }
                GL.EnableVertexAttribArray(attributes[varName].Location);
                GL.VertexAttribPointer(attributes[varName].Location, size, VertexAttribPointerType.Float, false,
                    Vertex.Size, offset);
            }
        }

        internal void DestroyShaders()
        {
            if (programId != 0)
                GL.DeleteProgram(programId);
        }

        internal void DisableAllVertexAttribArrays()
        {
            foreach (string varName in attributes.Keys)
                GL.DisableVertexAttribArray(attributes[varName].Location);
        }

        internal void EnableAllVertexAttribArrays()
        {
            foreach (string varName in attributes.Keys)
                GL.EnableVertexAttribArray(attributes[varName].Location);
        }

        internal bool LoadShaderCodes(string vCodePath, string fCodePath)
        {
            // Read the shader codes from files
            string vCode = ReadShaderCode(vCodePath),
                   fCode = ReadShaderCode(fCodePath);
            if (string.IsNullOrEmpty(vCode) || string.IsNullOrEmpty(fCode))
                return false;

            // Compile and load the shader codes
            int vertexShaderId = LoadShader(vCode, ShaderType.VertexShader),
                fragmentShaderId = LoadShader(fCode, ShaderType.FragmentShader);
            if (vertexShaderId == 0 || fragmentShaderId == 0)
                return false;

            // Create the program and then attach the compiled shaders to the program
            programId = GL.CreateProgram();
            GL.AttachShader(programId, vertexShaderId);
            GL.AttachShader(programId, fragmentShaderId);

            // Get the uniform and attributes from the code
            GL.LinkProgram(programId);
            int linkResult = 0;
            GL.GetProgram(programId, GetProgramParameterName.LinkStatus, out linkResult);
            if (linkResult == 0)
            {
                string errorInfo;
                GL.GetProgramInfoLog(programId, out errorInfo);
                Log.Write(LogLevel.ERROR, "Failed to link the shaders to the program: {0}", errorInfo);
                return false;
            }

            GL.ValidateProgram(programId);
            int validateResult = 0;
            GL.GetProgram(programId, GetProgramParameterName.ValidateStatus, out validateResult);
            if (validateResult == 0)
            {
                string errorInfo;
                GL.GetProgramInfoLog(programId, out errorInfo);
                Log.Write(LogLevel.ERROR, "Shader program validation failed: {0}", errorInfo);
                return false;
            }

            // For some reason, the progrma would crash if the codes in ShaderUseDict for getting all uniforms are used
            // So just omit them for now
#if ShaderUseDict
            int attributeCount = 0,
                uniformCount = 0;
            GL.GetProgram(programId, GetProgramParameterName.ActiveAttributes, out attributeCount);
            GL.GetProgram(programId, GetProgramParameterName.ActiveUniforms, out uniformCount);
#endif

            GL.UseProgram(programId);
            foreach (KeyValuePair<ShaderAttribute, string> attribPair in attributeVarNameMap)
            {
                AttributeVariable aVar = new AttributeVariable();
                int location = 0;
                if (attribPair.Key == ShaderAttribute.NORMAL)
                    location = 1;
                else if (attribPair.Key == ShaderAttribute.TEXCOORD)
                    location = 2;
                GL.BindAttribLocation(programId, location, attribPair.Value);
                aVar.Name = attribPair.Value;
                aVar.Location = location;
                aVar.Type = attribPair.Key == ShaderAttribute.TEXCOORD 
                    ? ActiveAttribType.FloatVec2 : ActiveAttribType.FloatVec3;
                attributes.Add(attribPair.Value, aVar);
            }

#if ShaderUseDict
            for (int i = 0; i < uniformCount; i++)
            {
                UniformVariable uVar = new UniformVariable();
                int length = 0;
                StringBuilder name = new StringBuilder();

                GL.GetActiveUniform(programId, i, MAX_VAR_LENGTH, out length, out uVar.Size, out uVar.Type, name);
                uVar.Name = name.ToString();
                uVar.Location = GL.GetAttribLocation(programId, uVar.Name);
                uniforms.Add(name.ToString(), uVar);
            }
#endif

            GL.UseProgram(0);

            // Delete the shaders as they are already in the program
            GL.DetachShader(programId, vertexShaderId);
            GL.DeleteShader(vertexShaderId);
            GL.DetachShader(programId, fragmentShaderId);
            GL.DeleteShader(fragmentShaderId);

            loadSuccess = true;
            return loadSuccess;
        }

        internal void SetUniform(string varName, int value)
        {
            int location = GetUniformLocation(varName);
            if (location >= 0)
                GL.Uniform1(location, value);
        }

        internal void SetUniform(string varName, float value)
        {
            int location = GetUniformLocation(varName);
            if (location >= 0)
                GL.Uniform1(location, value);
        }

        internal void SetUniform(string varName, Vector3 value)
        {
            int location = GetUniformLocation(varName);
            if (location >= 0)
                GL.Uniform3(location, value);
        }

        internal void SetUniform(string varName, Matrix3 value)
        {
            int location = GetUniformLocation(varName);
            if (location >= 0)
                GL.UniformMatrix3(location, false, ref value);
        }

        internal void SetUniform(string varName, Matrix4 value)
        {
            int location = GetUniformLocation(varName);
            if (location >= 0)
                GL.UniformMatrix4(location, false, ref value);
        }

        internal void SetUniform(ShaderUniform varName, Matrix4 value)
        {
            switch (varName)
            {
                case ShaderUniform.PROJECTION_MATRIX:
                    SetUniform("projectionMatrix", value);
                    break;
                case ShaderUniform.VIEW_MATRIX:
                    SetUniform("viewMatrix", value);
                    break;
                case ShaderUniform.MODEL_MATRIX:
                    SetUniform("modelMatrix", value);
                    break;
            }
        }

        internal void SetLight(string lightPos, string lightColor, string lightType, Light light)
        {
            SetUniform(lightPos, light.Position);
            SetUniform(lightColor, light.Color);
            SetUniform(lightType, (int)light.Type);
        }

        internal void UseProgram()
        {
            if (loadSuccess)
                GL.UseProgram(programId);
        }

        internal void UnUseProgram()
        {
            GL.UseProgram(0);
        }

        internal bool Loaded
        {
            get { return loadSuccess; }
        }

        private int GetAttribLocation(string varName)
        {
            if (!attributes.ContainsKey(varName))
                return -1;
            else
                return attributes[varName].Location;
        }

        private int GetUniformLocation(string varName)
        {
            if (!uniforms.ContainsKey(varName))
            {
                int location = GL.GetUniformLocation(programId, varName);
                if (location >= 0)
                {
                    UniformVariable uVar = new UniformVariable();
                    uVar.Name = varName;
                    uVar.Location = location;
                    uniforms.Add(varName, uVar);
                    return location;
                }
                else
                    return -1;
            }
            return uniforms[varName].Location;
        }

        private string ReadShaderCode(string codePath)
        {
            string code = string.Empty;
            try
            {
                code = File.ReadAllText(codePath);
                return code;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private int LoadShader(string code, ShaderType type)
        {
            int shaderHandle = GL.CreateShader(type);
            GL.ShaderSource(shaderHandle, code);
            GL.CompileShader(shaderHandle);

            int compileStatus;
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
            {
                string errorInfo;
                GL.GetShaderInfoLog(shaderHandle, out errorInfo);
                Log.Write(LogLevel.ERROR, "Failed to compile the shader: {0}", errorInfo);
                return 0;
            }
            return shaderHandle;
        }
    }
}
