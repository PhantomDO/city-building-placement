# Création d'une ville
Ce projet Unity permet la génération procédurale d'une ville, dont le placements des différents éléments constituants une ville. Vous trouverez dans le markdown le travail de recherche.

[Presentation pptx](https://1drv.ms/p/s!AtdMxyMzK-l-g_YaX9siiuPIIg3-tg?e=iuZEN4)

 ![image](/images-rapport/multipleCityGeneration.PNG "Exemple de generation")
# Publications et intuitions

## Travaux de recherches précédents
* [Generation procédurale de monde, Adrien Peytavie, 2013](https://tel.archives-ouvertes.fr/tel-00841373/document)
* [Map Generator : Create procedural American-style cities, Keir
"ProbableTrain", 2020](https://github.com/ProbableTrain/MapGenerator)

## Premières intuitions
La première intuition a été de baser la création de la ville sur une image représentant des routes. En utilisant l'[algorithme de remplissage par diffusion](https://fr.wikipedia.org/wiki/Algorithme_de_remplissage_par_diffusion), chaque quartier séparé par une route peut être nommé d'un type de zone.

 ![image](/city-building-placement/Assets/Map/road_lyon_128.jpg "Routes de Lyon")

Une autre intuition, plus simple, est basée sur une grille représentant une ville, dont on a rempli chaque case par la dénomination d'une zone (centre-ville, résidentiel, industriel...).
Puis, en utilisant l'algorithme [Marching square](https://fr.wikipedia.org/wiki/Marching_squares) (utilisation de l'algorithme [Marching square dans Unity](https://catlikecoding.com/unity/tutorials/marching-squares-series/)), on peut délimiter ces zones non plus selon la grille mais selon les coordonnées de la carte.

# Règles de générations
## Elements d'une ville
Selon nos connaissances et nos observations, plusieurs règles de générations ont été décidées pour le remplissage de la grille  :
* Une ville est de forme ronde.
* La zone "Centre-ville" est unique et elle est au centre de la ville.
* Autour du centre-ville, la zone "résidentiel" s'étend sur tout le reste de la ville.
* Une zone "industrielle" est forcément en dehors du centre-ville.
* Une zone de "bureau" est forcément en centre-ville.

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
    * Toutes les
    * Aléatoire, + grand en s'éloignant du centre ville (non implémenté)
  * Zone d'eau
    * Cours d'eau passant dans la ville
    * Dans les espaces verts (non implémenté)
 * Routes
   * Entre les batiments (séparés par des chunks de N * N)

De plus, les bâtiments présents dans une ville diffèrents selon leur emplacement. Ainsi, un batiment en centre ville sera un immeuble haut, et plus on s'écarte du centre ville, moins les bâtiments sont hauts. De la même façon, il y a plus de bâtiment au centre de la ville, et plus on s'éloigne moins la ville est dense.

# Distribution gausienne

Cette problématique nous a fait penser à une distribution gaussienne. Une [fonction gaussienne](https://fr.wikipedia.org/wiki/Fonction_gaussienne) a dont été utilisée afin d'instancier les bâtiments de la ville (plus dense au centre, plus étendu sur les bords).



plusieurs centre ville



# Generation des batiments en fonction des zones

# Generation de route simple

Pour compléter le vide entre les batiments, nous avons généré une grille entre les quartiers.
Étant donné le projet se focalise sur le placement, la grille des routes est très simple et sert de découpage de la grille en carré.

# Generation de parc

Parfois, on appercevait des quartiers entièrement vide, nous avons compléter ces quartiers par des parcs.

# Generation des zones industriels

Comme pour la génération

# Rendu Graphique

Pour afficher une grille avec le plus de mesh possible plusieurs options étaient disponible :

* Instanciation de prefab unity ([pool d'objet](https://camo.githubusercontent.com/639c3d03006cd247cc06b3fcc35efb20c2400e311a08d76b15674149f0d4d766/68747470733a2f2f6d65646961322e67697068792e636f6d2f6d656469612f6c5162466a396c3754326c41757954576a4d2f67697068792e676966))
![image](https://camo.githubusercontent.com/639c3d03006cd247cc06b3fcc35efb20c2400e311a08d76b15674149f0d4d766/68747470733a2f2f6d65646961322e67697068792e636f6d2f6d656469612f6c5162466a396c3754326c41757954576a4d2f67697068792e676966)
* Utilisation des gizmos pour afficher des primitives simples
![image](/images-rapport/drawWithGizmos.PNG "Affichage avec les gizmos")
* Utilisation du GPU instancing de mesh ([image de l'article](https://toqoz.fyi/assets/2019_compute_movement.gif))
![image](https://toqoz.fyi/assets/2019_compute_movement.gif)


La dernière options à été choisi. Un exemple de cette solution trouvé dans cette [article](https://toqoz.fyi/thousands-of-meshes.html) explique le principe de base d'utilisation ainsi qu'une demonstration d'un code déjà fonctionnel. Cependant pour rester le plus performant possible, nous sommes restés sur une solution avec des cubes en lieu et place de mesh avec plusieurs subMeshes.

Le principe de la méthode DrawMeshInstanceInrect est de charger le mesh sur le GPU afin de le l'instancier N fois dans un seul drawcall au lieu de faire appel à l'instanciation d'objet d'Unity (ne fusionne pas les drawcalls d'un même objet et plus lourd) pour seulement faire de l'affichage. Cependant, nous n'avons pas de physique sur ces objets. Cette méthode est un wrapper qui permet de ne pas toucher directement au GPU.

Cela passe par la création d'un [shader](city-building-placement/Assets/Material/Custom_InstancedIndirectColor.mat) qui va lire nos données envoyées (MeshProperties) afin d'afficher dans le moteur les meshs a leurs transformation et couleur respectives. Ce shader est utiliser via un material que nous passons à notre class de rendu.



[Exemple de structure contenant la couleur et la matrice de transformation de chacun des mesh qui doit être chargé](city-building-placement\Assets\Scripts\Rendering\CityRenderer.cs#L8)
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

[Ici un exemple d'initialisation des différents buffer pour chacun des bâtiments.](city-building-placement\Assets\Scripts\Rendering\CityRenderer.cs#L130)
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

[Ici la fonction d'affichage appelé pour chaque type de bâtiments.](city-building-placement\Assets\Scripts\Rendering\CityRenderer.cs#L55)
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


# Architecture

[Affichage de diagram puml dans github](https://stackoverflow.com/questions/32203610/how-to-integrate-uml-diagrams-into-gitlab-or-github/32771815#32771815)

## CityClass

![CityClass](http://www.plantuml.com/plantuml/png/TOv1giCm30Nt_nI_1uYrxoebtRVG4oYoY0MsR2oPqjlNEK1NmuFXCQivTXyb_rr4AO_8_yczsHMWupiSucdimxXJLMVrGBN0Cw5wBVS7aFXdsOVaA7eiBIDnONKeA0fpwddzqxgPiHkyg4kRtAQsPJGBxGnQMUFa5m00)

## CityBuild

![CityBuild](http://www.plantuml.com/plantuml/png/TSvD2WCX38RXVK_H7c3Clglfpxw5di29OGne51CflNtr0BDw2Jo4RrNppNfEfmiBfXxe7CsgNW6Q_xgTBVQjApKiOYJci6HuHozMylC5oFPRjiufiEnBJHuf3mNPiy0uPRu-L3TLCeKFDgwZZy7kKxaw1wqaOVGF)
## CityRenderer

![CityRenderer](http://www.plantuml.com/plantuml/png/TOv1hi8m30Nt_nIV0xJUiog2FGXEaBfMOobn8jiHulQu1s0r6cdqz1Rpf3vg-JknO1aR_MVtRcS0JUzvPyzZ7KQAJPp4PsmL7Zc9jtgx0x9_fdLmsLZsgPU4LAE2cbWWL8xfdzN5ZDpWXSezU8tiIRAHaixz-BI41Ry0)

# Résultats

![image](/images-rapport/city.PNG "Exemple de city genere")
![image](/images-rapport/withoutIndustrialZone.PNG "City without industrial zone")
![image](/images-rapport/multipleCityGeneration.PNG "Multiple city")
