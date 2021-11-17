# Spécifications
    Project Version : Unity 2020.3.22f1

  # Création d'une ville

Ce projet Unity permet la génération procédurale d'une ville, dont le placements des différents éléments constituants une ville. Vous trouverez dans le markdown le travail de recherche.

[Presentation pdf](M2_GENERATION_PROCEDURALE_VILLE_FEUILLASTRE_KOCH.pdf)

 ![image](/images-rapport/multipleCityGeneration.PNG "Exemple de generation")
# Publications et intuitions

## Travaux de recherches précédents
* [Generation procédurale de monde, Adrien Peytavie, 2013](https://tel.archives-ouvertes.fr/tel-00841373/document)
* [Map Generator : Create procedural American-style cities, Keir
"ProbableTrain", 2020](https://github.com/ProbableTrain/MapGenerator)

## Premières intuitions
La première intuition a été de baser la création de la ville sur une texture 2D représentant des routes. En utilisant l'[algorithme de remplissage par diffusion](https://fr.wikipedia.org/wiki/Algorithme_de_remplissage_par_diffusion), chaque quartier séparé par une route peut être nommé d'un type de zone.

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
    * Tous les quartiers qui n'ont pas de bâtiments
    * Aléatoire, + grand en s'éloignant du centre ville (non implémenté)
  * Zone d'eau
    * Cours d'eau passant dans la ville
    * Dans les espaces verts (non implémenté)

De plus, les bâtiments présents dans une ville diffèrents selon leur emplacement. Ainsi, un batiment en centre ville sera un immeuble haut, et plus on s'écarte du centre ville, moins les bâtiments sont hauts. De la même façon, il y a plus de bâtiment au centre de la ville, et plus on s'éloigne moins la ville est dense.

# Distribution gausienne

Cette problématique nous a fait penser à une distribution gaussienne. Une [fonction gaussienne](https://fr.wikipedia.org/wiki/Fonction_gaussienne) a dont été utilisée afin d'instancier les bâtiments de la ville (plus dense au centre, plus étendu sur les bords).

La génération de la ville (fonction [CreateEpicentres](city-building-placement/Assets/Scripts/CityBuild.cs#L147)) se fait en créant des cercles successifs jusqu'à la taille du rayon de la ville. Pour chacun des points de ce cercle, on le place dans la grille en lui donnant un type de zone (centre-ville si il est au centre, résidentiel sinon). Puis, selon la fonction gaussienne, la position a une certaine probabilité qu'elle devienne occupée, et qu'il y aura un bâtiment sur cette case.

![image](/images-rapport/loi-gaussienne.png "Remplissage avec la fonction gaussienne")

Jouer sur les différents paramètres de la classe de la ville (CityClass) permet de modifier les paramètres de la fonction gaussienne, et ainsi modifier la densité ou l'étendue de la ville.
Partant de cette façon de construire la ville, on peut instancier plusieurs ville dans le rendu.

![image](/images-rapport/inspecteurCity.png "Paramètre des villes")

# Génération des batiments en fonction des zones
On peut interpréter deux zones occupées collées de plusieurs façons :
- C'est un bâtiment plus grand : les deux zones occupées fusionnent ;
- C'est une zone dense, les bâtiments sont plus hauts.

Intuitivement, on compte le nombre de bâtiments dans la zone d'un certain rayon autour d'un bâtiment ; on supprime ces bâtiments et on agrandit proportionnellement le bâtiment central. Cette méthode, bien que particulièrement pertinente, s'est révélé lourde à implémenter.

Lors de la séparation de la carte en quartiers, nous avons appliqué une autre méthode ([CreateBuildings](city-building-placement/Assets/Scripts/CityBuild.cs#L201)), plus simple mais moins précise :
* Si une zone est complètement occupé, c'est qu'elle est très dense, les immeubles seront plus hauts ;
* Si la zone est remplie aux 2/3 ou plus, ce sont des immeubles moyens,
* Si la zone est remplie à 1/3 ou plus, ce sont de petits immeubles,
* Si la zone contient moins de 1/3 de zones occupées, ce sont des maisons,
* Sinon c'est un parc.



# Generation de route simple

Afin de structurer la ville, nous avons généré une grille de routes entre les quartiers de n*n (fonction [CreateRoads](city-building-placement/Assets/Scripts/CityBuild.cs#L293)) (taille et nombre des quartiers modifiables dans les paramètres du component CityBuild).
Puisque le projet se focalise principalement sur le placement des différents éléments d'une ville, la grille des routes est très simple et sert de découpage de la grille en quartiers carrés.

Plus tard, ce découpage en quartiers nous a permit de choisir les types de bâtiments selon le nombre de zones occupées dans le quartier.

# Generation de parc et de rivière
Les parcs sont de grands espaces sans bâtiments. Ainsi, les quartiers qui n'ont pas de bâtiments deviennent des parcs.

Souvent, une rivière traverse une ville. Ainsi, crée dans la fonction [CreateRiver](city-building-placement/Assets/Scripts/CityBuild.cs#L272), la rivière suit une courbe de bézier, le premier point étant une position aléatoire sur l'un des côté de la carte, les points au milieu sont les positions des épicentres des villes, et le dernier points est une position aléatoire de l'autre côté de la carte.

# Generation des zones industriels
Les zones industrielles ne peuvent pas être placées dans le centre-ville.

Pendant la création de la zone résidentielle, la zone attribuée à la case de la grille a une très faible chance de se transformer en quartier industriel.
Puis, on génère autour de ce point une zone industrielle avec un très faible radius (fonction [CreateIndustrialZone](city-building-placement/Assets/Scripts/CityBuild.cs#L180)) puis dans la génération de batiments on place des usines dans les zones.

# Rendu Graphique

Pour afficher une grille avec le plus de mesh possible plusieurs options étaient disponible :

* Instanciation de prefab unity ([pool d'objet](https://camo.githubusercontent.com/639c3d03006cd247cc06b3fcc35efb20c2400e311a08d76b15674149f0d4d766/68747470733a2f2f6d65646961322e67697068792e636f6d2f6d656469612f6c5162466a396c3754326c41757954576a4d2f67697068792e676966))

![image](https://camo.githubusercontent.com/639c3d03006cd247cc06b3fcc35efb20c2400e311a08d76b15674149f0d4d766/68747470733a2f2f6d65646961322e67697068792e636f6d2f6d656469612f6c5162466a396c3754326c41757954576a4d2f67697068792e676966)
* Utilisation des gizmos pour afficher des primitives simples

![image](/images-rapport/drawWithGizmos.PNG "Affichage avec les gizmos")
* Utilisation du GPU instancing de mesh

![image](https://thumbs.gfycat.com/HatefulNearBuckeyebutterfly-size_restricted.gif)


La dernière option à été choisie. Un exemple de cette solution, trouvée dans cette [article](https://toqoz.fyi/thousands-of-meshes.html), explique le principe de base d'utilisation ainsi qu'une demonstration d'un code déjà fonctionnel. Cependant pour rester le plus performant possible, nous sommes restés sur une solution avec des cubes en lieu et place de mesh avec plusieurs subMeshes.

Le principe de la méthode DrawMeshInstanceInrect est de charger le mesh sur le GPU afin de l'instancier N fois dans un seul drawCall au lieu de faire appel à l'instanciation d'objet d'Unity (ne fusionne pas les drawcalls d'un même objet et plus lourd) pour seulement faire de l'affichage. Cependant, nous n'avons pas de physique sur ces objets. Cette méthode est un wrapper qui permet de ne pas toucher directement au GPU.

Cela passe par la création d'un [shader](city-building-placement/Assets/Material/Custom_InstancedIndirectColor.mat) qui va lire nos données envoyées (MeshProperties) afin d'afficher dans le moteur les meshs a leurs transformation et couleur respectives. Ce shader est utilisé via un material passé à la classe de rendu.

<br/>

[Exemple de structure contenant la couleur et la matrice de transformation de chacun des mesh qui doit être chargé](city-building-placement/Assets/Scripts/Rendering/CityRenderer.cs#L8)
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

[Ici un exemple d'initialisation des différents buffer pour chacun des bâtiments.](city-building-placement/Assets/Scripts/Rendering/CityRenderer.cs#L130)
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

[Ici la fonction d'affichage appelé pour chaque type de bâtiments.](city-building-placement/Assets/Scripts/Rendering/CityRenderer.cs#L55)
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
