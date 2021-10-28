using UnityEngine;

namespace Assets.Scripts
{
    public class DrawCityInstancedIndirect : MonoBehaviour
    {
        public uint SizeGrid = 128;

        public Material Material;
        public ComputeShader ComputeShader;
        private ComputeBuffer m_MeshPropertiesBuffer;
        private ComputeBuffer m_ArgsBuffer;

        private Mesh m_Mesh;
        private Bounds m_Bounds;

        private struct MeshProperties
        {
            public Matrix4x4 mat;
            public Vector4 color;

            public static int Size()
            {
                return sizeof(float) * 4 * 4 + sizeof(float) * 4;
            }
        }

        public void Setup()
        {
            m_Mesh = CreateCube();
            m_Bounds = new Bounds(transform.position, Vector3.one);
            InitializeBuffers();
        }

        public void InitializeBuffers()
        {
            int kernel = ComputeShader.FindKernel("CSMain");

            uint[] args = new uint[5] {0, 0, 0, 0, 0};

            // 0 = count of triangles indices
            // 1 = population
            // other when submeshes 
            Mesh mesh = CreateCube();
            args[0] = (uint) mesh.GetIndexCount(0);
            args[1] = (uint) SizeGrid * SizeGrid;
            args[2] = (uint) mesh.GetIndexStart(0);
            args[3] = (uint) mesh.GetBaseVertex(0);

            m_ArgsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            m_ArgsBuffer.SetData(args);

            // Init buffer with grid
            MeshProperties[] properties = new MeshProperties[SizeGrid * SizeGrid];
            for (int y = 0; y < SizeGrid; y++)
            {
                for (int x = 0; x < SizeGrid; x++)
                {
                    MeshProperties props = new MeshProperties();
                    Vector3 position = new Vector3(x, y, 0);
                    Quaternion rotation = Quaternion.identity;
                    Vector3 scale = Vector3.one;

                    props.mat = Matrix4x4.TRS(position, rotation, scale);
                    props.color = Color.white;
                }
            }

            m_MeshPropertiesBuffer = new ComputeBuffer((int)(SizeGrid * SizeGrid), MeshProperties.Size());
            m_MeshPropertiesBuffer.SetData(properties);

            ComputeShader.SetBuffer(kernel, "_Properties", m_MeshPropertiesBuffer);
            Material.SetBuffer("_Properties", m_MeshPropertiesBuffer);
        }

        public void Start()
        {
            Setup();
        }

        public void Update()
        {
            int kernel = ComputeShader.FindKernel("CSMain");
            ComputeShader.Dispatch(kernel, Mathf.CeilToInt((SizeGrid * SizeGrid) / 64f), 1, 1);
            Graphics.DrawMeshInstancedIndirect(m_Mesh, 0, Material, m_Bounds, m_ArgsBuffer);
        }

        public void OnDisable()
        {
            if (m_MeshPropertiesBuffer != null) m_MeshPropertiesBuffer.Release();
            m_MeshPropertiesBuffer = null;
            if (m_ArgsBuffer != null) m_ArgsBuffer.Release();
            m_ArgsBuffer = null;
        }

        public Mesh CreateCube(float width = 1f, float height = 1f, float depth = 1f)
        {
            var mesh = new Mesh();
            float w = width * .5f;
            float h = height * .5f;
            float d = depth * .5f;

            var vertices = new Vector3[]
            {
                new Vector3(-w, -h, -d),
                new Vector3(-w, h, -d),
                new Vector3(w, h, -d),
                new Vector3(w, -h, -d),
                new Vector3(-w, -h, d),
                new Vector3(-w, h, d),
                new Vector3(w, h, d),
                new Vector3(w, -h, d)
            };

            var triangles = new int[]
            {
                0,1,2,
                0,2,3,

                1,5,6,
                6,2,1,

                7,4,0,
                3,7,0,

                4,5,0,
                5,1,0,

                2,6,7,
                7,3,2,

                6,5,4,
                7,6,4
            };

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}