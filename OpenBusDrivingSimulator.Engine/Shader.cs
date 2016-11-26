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
                    Vertex.Size, Vector3.SizeInBytes * offset);
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

        internal int GetAttribLocation(string varName)
        {
            if (!attributes.ContainsKey(varName))
                return -1;
            else
                return attributes[varName].Location;
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
            int attributeCount = 0,
                uniformCount = 0;
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
            GL.GetProgram(programId, GetProgramParameterName.ActiveAttributes, out attributeCount);
            GL.GetProgram(programId, GetProgramParameterName.ActiveUniforms, out uniformCount);

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
            GL.UseProgram(0);

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
            if (uniforms.ContainsKey(varName))
                GL.Uniform1(uniforms[varName].Location, value);
        }

        internal void SetUniform(string varName, float value)
        {
            if (uniforms.ContainsKey(varName))
                GL.Uniform1(uniforms[varName].Location, value);
        }

        internal void SetUniform(string varName, Vector3 value)
        {
            if (uniforms.ContainsKey(varName))
                GL.Uniform3(uniforms[varName].Location, value);
        }

        internal void SetUniform(string varName, Matrix3 value)
        {
            if (uniforms.ContainsKey(varName))
            {
                float[] matrix = new float[9]
                {
                    value.M11, value.M12, value.M13,
                    value.M21, value.M22, value.M23,
                    value.M31, value.M32, value.M33
                };
                GL.UniformMatrix3(uniforms[varName].Location, 1, false, matrix);
            }
        }

        internal void SetUniform(string varName, Matrix4 value)
        {
            if (uniforms.ContainsKey(varName))
            {
                float[] matrix = new float[16]
                {
                    value.M11, value.M12, value.M13, value.M14,
                    value.M21, value.M22, value.M23, value.M24,
                    value.M31, value.M32, value.M33, value.M34,
                    value.M41, value.M42, value.M43, value.M44
                };
                GL.UniformMatrix4(uniforms[varName].Location, 1, false, matrix);
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
