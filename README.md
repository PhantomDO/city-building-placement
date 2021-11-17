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

<html>
    <!-- <style type="text/css">
    ul{color:#fff;background-color:#202020;margin:0;padding:1}
    ul{list-style-type:none;}
    ul ul{padding-left:16px}
    .t0{color:#808080;}
    .t1{color:#9400d3;}
    .i0{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAABZSURBVDhPYxhwwAilGb58+fIfyiQK8PDwgPWiGMAbswrKQwWfl4QxIMuB+DADmMAiFACKDSA6DIjyAkgRDBMLqOsFdGfiAwSjET3qkMEQjUZ0APPCQAMGBgBYHC1MPBIGNQAAAABJRU5ErkJggg==') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i1{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAH9SURBVDhPYxjc4MuXLwZAfBiIrwNxyadPn0SgUvgBULHE169fZ9958vp3bN/+/3aVW/4v2HPt/5v3H78D5ZZ//vzZAaqUgRFKg8H///85gJIFP//+r5i09Sb/tO23GGJs5Rg0ZPgYlh5+yHDj2ReGGDsFhjgHJQY1Sd4bPDw8mnADgM4LYWJial9z4pFKw8rLDHpyfAw1QZoMimLcYPlXn34yZM05x3D92WcGFQlehl21TgxAAxjBBgCdtf/cvXcOFUsuMLz/8pOhO1aPwVxFCKzx559/DFN33mWYu/8Bg4YUL8PZe+8Yvv38y/B5SRjYABawKgYGh5y5Zxm42JkZHr/7zjBrzz2Gr0BFH7/9ZmhZe41BSpibgY2ZieHw9ddQ5QgAc8F/3phVYAFBHjYGbRl+hifvvjFwsDIzMDMxMlx9/BEshwxgLmCC8uHg/ZdfDEduvGZ48Oorw42nnzA0iwtwMExPMwUF+AsQH8MAXICDjZmhPECL4UKP549Ye6WWb9++qYLEwV4ARt3zIzfeSMzbd5dhy9mnDD9+/QUJw0GIpRxDQ5gug7wo9wpmZuZKTk7OB1ApiAFA57AADQlgZGRMfvfll8eSQw8YFh28x8DDwcrQEWPAYKEqfAaoppCPj+8IWBc+8P37dwVgKmwHuQqEgQGcAJUalICBAQDLo+Tw6diwuwAAAABJRU5ErkJggg==') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i2{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAHkSURBVDhPY6AZ+Pjxo8WXL18Of/78+TyQ9oAKYwBGKA0HHz58UGFhYWn+8OpLxPZZZxi+f/nJ4JNpziAmL7AHKF3Kw8NzAaISAuAGvH//XgCosfrb55+5+5deZD+48jLD759/wHJMzEwM5t7qDB6pJn8ERHmW/P//vxJo0AuQHNwAoDPfv37yQWBCygaGrx9/QEVRASs7C0P2VB8GeS3xH7y8vJwgMSawDAQIfPn4nSFrqjeDoYsyVAgBVE2kGbKmeDGwc7EwMDIyckCFUVzwv8ByBoOKsSRDaJktw5cP3xm2zzwLDINfDN4ZJgwSyoIMGyeeZLi47x7DhOMZDEAvgPViGAADxu6qDL65ZgwsrEwMuxdcYDi4/DJUhoE4A/ABZAOQw4Ag0HdUYug7kgblQQDcAGDU/LEP1wOHNDYA0uxbaMRQW1fL8O7dOwZgAisAicO9AE1A7W+efArZNusUw7ndd6AyUM1Fxgz5+XkMjx49YpCTk2OYPHkyg6CgYCHcABj49OmTDRMTU/ej668sNk05AY6FkgXBQM35DOfPn4eqYmAwNDRkmDRpEpSHBQCdGAEM2NtA+jmQXnXt2rX/zs7O/4EawTSIDxTvhyonDEB+BmkKCwsjXTMMgAwBRTdCMwMDAONz33oPIkKiAAAAAElFTkSuQmCC') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i3{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAFTSURBVDhPlVGxSgNBEN0sweaOM4WFudJCYmNrYZoU+h3+hRiwEvIFfoD5AVtBkBQBQUubHCnEKuZAC++uUrj1vb3d5e5IzPlgbmdn5r25mRV1xHHsZ1k2hc1guybcDJb89nCtaP8SKZMfz0NtTUQkPyR7nnf38XTTX9yPdIKgj1gP7sSKmEZOUK4jW3x/LYRSqpPnecfWIuwEW3Cm68g7R2ciPB2aWwHU6hO5VxzHbfrsUgfJLLYEgmKlRnuomXAHAyQiEixs58PLub7zrJHdaNL3/SXuToTWPblYYuaDorRA+S9tDQQGJiQElwKbpWn6niQJN8+Yfk6ezM1vh4qGmh/U9Fmjn5Gwf0LVIAiiIlpBj2NsbYfi83ncllKO2bRlkivBzi9X++ZWXSJ9jBJtFDCufo3yEu0e3AirgLHYoAuLNi7xL3DW2hLdohvDilTJQvwC+T4YscOlfBgAAAAASUVORK5CYII=') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i4{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAH8SURBVDhPYxjc4MuXLwZAfBiIrwNxyadPn0SgUvgBULHE169fZ9958vp3bN/+/3aVW/4v2HPt/5v3H78D5ZZ//vzZAaqUgRFKg8H///85gJIFP//+r5i09Sb/tO23GGJs5Rg0ZPgYlh5+yHDj2ReGGDsFhjgHJQY1Sd4bPDw8mnADgM4LYWJial9z4pFKw8rLDHpyfAw1QZoMimLcYPlXn34yZM05x3D92WcGFQlehl21TgxAAxjBBgCde+bj11/GfFysDBcfvAc65S+DmgRE488//xim7rzLMHf/AwYNKV6Gs/feMXz7+Zfh85IwsAFMIEVApxvv2LKewdbWluHmqd0MogLcDPuuvmZYf/oZg1XNPob9194wsDEzMRy+/hqsGRmAXQAMmP8gzYcPHwYbAqJ1i7YycLAyMzAzMTJcffwRrBgZoLgAG3jw6ivDjaefMDSLC3AwTE8zBbn6BYjPCLIdLIMF8MasgrIYGDjYmBnyvdQZCnzUf/BysvUA9XWKi4t/ARsAcjY6AHkDZkCIpRxDQ5gug7wo9wpmZuZKTk7OB2AJIEDxwq5du6AsCDBSEmLYVefEMC/L/IysMKct0M+RyJpBAGcYgMCBRucXlmoiiby8vKZ8fHxHoMIoAMUANzc3KAsCgBolgbYugHKxAryBCIomKBMHYGAAAEs60gQgq1jOAAAAAElFTkSuQmCC') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i5{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAIZSURBVDhPzVJNaBNREP52Q6XYJJoQJUpZNNTYHpTGomAhpbQHryI2CEbBa8FSSw4KqW5OejLFoBcVRBRRhF5a1FUaYS/iH4g0SUUlRg2lKYZsskrU+HzzkshGrWc/GGb2fTPffPt28X+jUqn08tB5pHlEDMPwNKh/gzd7TdO8+PpD4duhs0k2cGKGXXmQYsvF0hfO3SiXy4ONVkiNLMAYa+fkeLXGjp+bXVhz4c4rhIMKujuduK6/QyZfQXhgEw4P+uDf4MjY7faeXwLc3n5Zlk/ffpTrUm++xHbFiei+Hmxe3yH4JaOK0UvPkc6X0eV1QJscAheQhAC3+7Rkfu1zrm7Di2yRW6nB760PVr//wPl7b3A5mUX3Rgeevf2Ez9UaytdCQkCmJm697+7MNILBIBYe38e6tR2Ymy9g+kke/dE5JFPLWGWToacLYtgK4YBfDKNhXdeFCOVtE7Nob7PBJkuYf18SzVa0OPgbsksmMh+NluG9uzpRvDqCZGyYXC/SmUzbqaCt1kwbrBjpVxA/uBWnJqPwuSQSmKJzqWn/d5CQI3xL1GfCvTiw04OxsaPI5XJQFAWJRAIul+tYyytomtao6tjhc0M7OYTRPVtYLKaKYQJlVVUhSVJ8xTsgPIwNL+72e47wciISicDtdotzyvTMMSW+QCAQ+COad9ME/aGpVIqFQiFGmfNxQVDjSiEaLCCRBlcfBvATyDAl6jXgjvAAAAAASUVORK5CYII=') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i6{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAHOSURBVDhPY6AZ+Pjxo8WXL18Of/78+TyQ9oAKYwBGKA0HHz58UGFhYWn+8OpLxPZZZxi+f/nJ4JNpziAmL7AHKF3Kw8NzAaISAuAGvH//XgCosfrb55+5+5deZD+48jLD759/wHJMzEwM5t7qDB6pJn8ERHmW/P//vxJo0AuQHNwAoDPfv37yQWBCygaGrx9/QEVRASs7C0P2VB8GeS3xH7y8vJwgMSawDAQIfPn4nSFrqjeDoYsyVAgBVE2kGbKmeDGwc7EwMDIyckCFIS74+vWrCdBZp0Hs5/feMXDwsDK8e/GJYfvMs8Aw+MXgnWHCIKEsyLBx4kmGi/vuMUw4nsEA9AJYL9gF//79a161ahWDra0tw+Ezexh+f//LcGzdDYaYJkeGzMmeDDdPP2Vo8l8O1owOwKYA/f8frPnwYYghQLrAcgZYATaA4QJKACPIdigbA6C7AhSdph5qDJE1DnAXsIAIkLPRAcgbyEDTQg6YoMwYpNVEDgC5hRBRNC/s2rULyoIAFSMpkAaGrEk+DOn9XjeAMeELtNkRiBGpEeQFQ0NDMH79+jWcDRIH4tvAvPD806dPGcBoBrsWHYDDAJcXYP7EB/AGImEDGBgARKXDaKoEC34AAAAASUVORK5CYII=') no-repeat;width:16px;height:16px;background-size: 16px 16px;}.i7{display:inline-block;vertical-align:text-bottom;margin-right:2px;background:url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAGcSURBVDhPY6AZ+Pjxo8WXL18Of/78+TyQ9oAKYwBGKA0HHz58UGFhYWn+8OpLxPZZZxi+f/nJ4JNpziAmL7AHKF3Kw8NzAaISAuAGvH//XgCosfrb55+5+5deZD+48jLD759/wHJMzEwM5t7qDB6pJn8ERHmW/P//vxJo0AuQHNwAoDPfv37yQWBCygaGrx9/QEVRASs7C0P2VB8GeS3xH7y8vJwgMSawDAQIfPn4nSFrqjeDoYsyVAgBVE2kGbKmeDGwc7EwMDIyckCFUVzwv8ByBoOKsSRDaJktw5cP3xm2zzwLDINfDN4ZJgwSyoIMGyeeZLi47x7DhOMZDEAvgPViGAADxu6qDL65ZgwsrEwMuxdcYDi4/DJUhoE4A/ABZAOQw4AsADcAGDV/7MP1wCGNC0CiUwPKgwC4F6AJqP3Nk08h22adYji3+w5UBgI0LeSACcqMQVpN5ACQW4ieoODg06dPNsDwOH7t9L3/HfHL/tcHL/h/bv+N/0Cx68Dk7QNVRhgA80AEUNNtIP0caGgG0Iu4/TaAgIEBADN1r/lEAtIwAAAAAElFTkSuQmCC') no-repeat;width:16px;height:16px;background-size: 16px 16px;}
    </style> -->
    <ul style="elements">
      <ul>
        <li>
          <i class="i3" tooltip="public class"></i>
          <span>CityRenderer </span>
          <span class="t0">(in Assets.Scripts)</span>
        </li>
        <ul>
          <li>
            <i class="i1" tooltip="public serialised field"></i>
            <span>buildingMeshes:</span>
            <span class="t1">GenericDictionary&lt;Building,Mesh&gt;</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i1" tooltip="public serialised field"></i>
            <span>buildingMaterials:</span>
            <span class="t1">GenericDictionary&lt;Building,List&lt;Material&gt;&gt;</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i4" tooltip="private field"></i>
            <span>_bounds:</span>
            <span class="t1">Bounds</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i1" tooltip="public serialised field"></i>
            <span>drawGizmos:</span>
            <span class="t1">bool</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i4" tooltip="private field"></i>
            <span>_builder:</span>
            <span class="t1">CityBuild</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i1" tooltip="public field"></i>
            <span>material:</span>
            <span class="t1">Material</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i1" tooltip="public field"></i>
            <span>computeShader:</span>
            <span class="t1">ComputeShader</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i4" tooltip="private field"></i>
            <span>_meshPropertiesBuffer:</span>
            <span class="t1">ComputeBuffer</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i4" tooltip="private field"></i>
            <span>_argsBuffers:</span>
            <span class="t1">GenericDictionary&lt;Building,List&lt;ComputeBuffer&gt;&gt;</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i5" tooltip="private static readonly field"></i>
            <span>Properties:</span>
            <span class="t1">int</span>
          </li>
          <li>
            <i class="i4" tooltip="private field"></i>
            <span>_isSetup:</span>
            <span class="t1">bool</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i7" tooltip="public method"></i>
            <span>Setup():</span>
            <span class="t1">void</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i7" tooltip="public method"></i>
            <span>InitializeBuffers():</span>
            <span class="t1">void</span>
          </li>
        </ul>
        <ul>
          <li>
            <i class="i7" tooltip="public method"></i>
            <span>CreateCube(float width, float height, float depth):</span>
            <span class="t1">Mesh</span>
          </li>
        </ul>
      </ul>
        <br>
  </ul>
</html>


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
## CityClass


## CityBuilder
## CityRenderer

# Résultats

# Conclusion
