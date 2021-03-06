using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public struct MeshProperties
    {
        public Matrix4x4 mat;
        public Vector4 color;

        public static int Size()
        {
            return sizeof(float) * 4 * 4 + sizeof(float) * 4;
        }
    }

    [RequireComponent(typeof(CityBuild))]
    public class CityRenderer : MonoBehaviour
    {
        public GenericDictionary<Building, Mesh> buildingMeshes;
        public GenericDictionary<Building, List<Material>> buildingMaterials;
        private Bounds _bounds;

        public bool drawGizmos = false;

        [Header("Component")]
        private CityBuild _builder;

        [Header("Shader")] 
        public Material material;
        public ComputeShader computeShader;
        private ComputeBuffer _meshPropertiesBuffer;
        private GenericDictionary<Building, List<ComputeBuffer>> _argsBuffers;

        private static readonly int Properties = Shader.PropertyToID("_Properties");

        private bool _isSetup;

        #region Unity

        private void Awake()
        {
            _builder = GetComponent<CityBuild>();
            _isSetup = false;

            _argsBuffers = new GenericDictionary<Building, List<ComputeBuffer>>();
            foreach (Building value in Enum.GetValues(typeof(Building)))
            {
                _argsBuffers.Add(value, new List<ComputeBuffer>());
            }
        }

        public void Update()
        {
            //int kernel = computeShader.FindKernel("CSMain");
            //computeShader.Dispatch(kernel, Mathf.CeilToInt((_builder.DimensionSize * _builder.DimensionSize) / 64f), 1, 1);

            if (_isSetup)
            {
                foreach (var mesh in buildingMeshes)
                {
                    for (int i = 0; i < mesh.Value.subMeshCount; i++)
                    {
                        Graphics.DrawMeshInstancedIndirect(mesh.Value, i, material, _bounds, _argsBuffers[mesh.Key][i]);
                    }
                }
            }
        }

        public void OnDisable()
        {
            _meshPropertiesBuffer?.Release();
            _meshPropertiesBuffer = null;

            if (_argsBuffers == null) return;

            foreach (var a in _argsBuffers)
            {
                foreach (var buffer in a.Value)
                {
                    buffer.Release();
                }

                a.Value.Clear();
            }
        }
        
        void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            if (_builder == null || _builder.DimensionSize <= 0) return;

            for (int i = 0; i < _builder.DimensionSize; i++)
            {
                for (int j = 0; j < _builder.DimensionSize; j++)
                {
                    int index = i * _builder.DimensionSize + j;

                    if (_builder.MapCase[index].occupied == true)
                    {
                        Gizmos.color = _builder.buildingColor[_builder.MapCase[index].building];
                        Gizmos.DrawCube(new Vector3(i, 0, j), CityCase.GetBuildingSize(_builder.MapCase[index]));
                    }
                }
            }
        }

        #endregion

        public void Setup()
        {
            if (buildingMeshes == null || buildingMeshes.Count <= 0)
            {
                Mesh mesh = CreateCube();
                foreach (var m in buildingMeshes)
                {
                    buildingMeshes[m.Key] = mesh;
                }
            }

            _bounds = new Bounds(transform.position, Vector3.one * _builder.DimensionSize * 4);

            InitializeBuffers();
            _isSetup = true;
        }

        public void InitializeBuffers()
        {
            int kernel = computeShader.FindKernel("CSMain");

            uint[] args = new uint[5] {0, 0, 0, 0, 0};

            // 0 = count of triangles indices
            // 1 = population
            // other when submeshes 
            foreach (var building in buildingMeshes)
            {
                for (int i = 0; i < building.Value.subMeshCount; i++)
                {
                    args[0] = (uint)building.Value.GetIndexCount(i);
                    args[1] = (uint)(_builder.DimensionSize * _builder.DimensionSize);
                    args[2] = (uint)building.Value.GetIndexStart(i);
                    args[3] = (uint)building.Value.GetBaseVertex(i);

                    _argsBuffers[building.Key].Add(new ComputeBuffer(1, 
                        args.Length * sizeof(uint), ComputeBufferType.IndirectArguments));
                    _argsBuffers[building.Key][i].SetData(args);
                }
            }

            // Init buffer with grid
            MeshProperties[] properties = new MeshProperties[_builder.DimensionSize * _builder.DimensionSize];
            for (uint z = 0; z < _builder.DimensionSize; ++z)
            {
                for (uint x = 0; x < _builder.DimensionSize; ++x)
                {
                    var cityCase = _builder.MapCase[z * _builder.DimensionSize + x];

                    MeshProperties props = new MeshProperties();
                    Vector3 scale = CityCase.GetBuildingSize(cityCase);
                    Vector3 position = new Vector3(x, scale.y / 2, z);
                    Quaternion rotation = Quaternion.identity;

                    props.mat = Matrix4x4.TRS(position, rotation, scale);
                    props.color = cityCase.occupied == true ? _builder.buildingColor[cityCase.building] : Color.clear;
                    properties[z * _builder.DimensionSize + x] = props;
                }
            }

            _meshPropertiesBuffer = new ComputeBuffer((int)(_builder.DimensionSize * _builder.DimensionSize), MeshProperties.Size());
            _meshPropertiesBuffer.SetData(properties);

            //computeShader.SetBuffer(kernel, "_Properties", _meshPropertiesBuffer);
            material.SetBuffer("_Properties", _meshPropertiesBuffer);
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