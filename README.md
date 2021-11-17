# Création d'une ville
Ce projet Unity permet la génération procédurale d'une ville, dont le placements des différents éléments constituants une ville. Vous trouverez dans le markdown le travail de recherche.

 ![image](/images-rapport/multipleCityGeneration.PNG "Exemple de generation")
# Publications et pistes de recherche

*  [Algorithme de remplissage par diffusion](https://fr.wikipedia.org/wiki/Algorithme_de_remplissage_par_diffusion)
* [Thèse Generation procédurale de monde](https://tel.archives-ouvertes.fr/tel-00841373/document)
* [Courbe gaussiene](https://fr.wikipedia.org/wiki/Fonction_gaussienne)
* [Marching square](https://fr.wikipedia.org/wiki/Marching_squares)
* [Marching square in Unity](https://catlikecoding.com/unity/tutorials/marching-squares-series/)
* [Map Generator (american city)](https://github.com/ProbableTrain/MapGenerator)

## Premières pistes

plusieurs centre ville
utiliser une courbe gaussienne pour la densité des maisons/hauteur des batiments
height map


# Content

Nous avons découpé ces élements en zone et rempli notre grille par des batiments/parc/eau, etc...

Voici les paramètres des zones :

  * Centre ville
    * Au centre de la ville
    * Radius modulable
  * Quartier résidentiel
    * S'étend autour du centre-ville
  * Zone industriel
    * Au abord de la ville, dans la périphérie
    * Généré dans les quartiers résidentiels
  * Zone travail (entreprise)
    * Non implémentée
  * Espace vert
    * Aléatoire, + grand en s'éloignant du centre ville
    * Radius : 0.2 - 1.0
    * Area *= Radius
  * Zone d'eau
    * Dans les espaces verts
    * Radius : 0.2 - 1.0
    * Area *= Radius
 * Routes
   * Entre les batiments (séparés par des chunks de N * N)

# Contexte


# Rendu Graphique

Pour afficher une grille avec le plus de mesh possible, nous avons optez pour une option disponible dans le moteur Unity, le mesh instancing.
Nous avons trouvez cette solution dans cette [article](https://toqoz.fyi/thousands-of-meshes.html) qui explique le principe de base d'utilisation ainsi qu'une demo d'un code déjà fonctionnel. Nous avons pu adaptez ce code pour afficher des meshs avec plusieurs subMeshes. Cependant pour rester le plus performant possible nous sommes rester sur une solution avec des cubes (pour les captures d'écran).

Le principe de la méthode DrawMeshInstanceInrect est de charger le mesh sur le GPU afin de le l'instantier N fois dans un seul drawcall au lieu de faire appel à l'instantiation d'object d'Unity (ne fusionne pas les drawcalls d'un même object et plus lourd) pour seulement faire de l'affichage. Cependant, nous n'avons pas de physique sur ces objets. Cette méthode est un wrapper qui permet de ne pas toucher au GPU.

Exemple de structure contenant la couleur et la matrice de transformation de chacun des mesh qui doit être chargé
```cs
public struct MeshProperties
{
  public Matrix4x4 mat;
  public Vector4 color;

  // usefull to get to total size of the struct
  public static int Size()
  {
      return sizeof(float) * 4 * 4 + sizeof(float) * 4;
  }
}
```

Ici un exemple d'initialisation des différent buffer pour chacun de nos batiments.
```cs
public void InitializeBuffers()
{
    uint[] args = new uint[5] {0, 0, 0, 0, 0};

    // 0 = count of triangles indices
    // 1 = number of mesh to draw
    // 2 = submesh starting index
    // 3 = submesh base offset index
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
            props.color = cityCase.occupied == true
              ? _builder.buildingColor[cityCase.building] : Color.clear;
            properties[z * _builder.DimensionSize + x] = props;
        }
    }

    _meshPropertiesBuffer = new ComputeBuffer(
        (int)(_builder.DimensionSize * _builder.DimensionSize),
        MeshProperties.Size());

    // give all the properties generated before to the struct in the shader
    _meshPropertiesBuffer.SetData(properties);
    material.SetBuffer("_Properties", _meshPropertiesBuffer);
}
```

Ici la fonction qui est appelé pour chaque type de batiments. (Update)
```cs
foreach (var mesh in buildingMeshes)
{
    for (int i = 0; i < mesh.Value.subMeshCount; i++)
    {
        Graphics.DrawMeshInstancedIndirect(mesh.Value, i,
              material, _bounds, _argsBuffers[mesh.Key][i]);
    }
}
```




# Definition

# Architecture

[Affichage de diagram puml dans github](https://stackoverflow.com/questions/32203610/how-to-integrate-uml-diagrams-into-gitlab-or-github/32771815#32771815)

## CityClass

![CityClass](http://www.plantuml.com/plantuml/png/TOv1giCm30Nt_nI_1uYrxoebtRVG4oYoY0MsR2oPqjlNEK1NmuFXCQivTXyb_rr4AO_8_yczsHMWupiSucdimxXJLMVrGBN0Cw5wBVS7aFXdsOVaA7eiBIDnONKeA0fpwddzqxgPiHkyg4kRtAQsPJGBxGnQMUFa5m00)

## CityBuild

![CityBuild](http://www.plantuml.com/plantuml/png/TSvD2WCX38RXVK_H7c3Clglfpxw5di29OGne51CflNtr0BDw2Jo4RrNppNfEfmiBfXxe7CsgNW6Q_xgTBVQjApKiOYJci6HuHozMylC5oFPRjiufiEnBJHuf3mNPiy0uPRu-L3TLCeKFDgwZZy7kKxaw1wqaOVGF)
## CityRenderer

![CityRenderer](http://www.plantuml.com/plantuml/png/TOv1hi8m30Nt_nIV0xJUiog2FGXEaBfMOobn8jiHulQu1s0r6cdqz1Rpf3vg-JknO1aR_MVtRcS0JUzvPyzZ7KQAJPp4PsmL7Zc9jtgx0x9_fdLmsLZsgPU4LAE2cbWWL8xfdzN5ZDpWXSezU8tiIRAHaixz-BI41Ry0)

# Résultats

# Conclusion
