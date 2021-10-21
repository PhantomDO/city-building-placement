# city-building-placement

## Source

*  [Algorithme de remplissage par diffusion](https://fr.wikipedia.org/wiki/Algorithme_de_remplissage_par_diffusion)
* [Thèse Generation procédurale de monde](https://tel.archives-ouvertes.fr/tel-00841373/document)
* [Courbe gaussiene](https://fr.wikipedia.org/wiki/Fonction_gaussienne)
* [Marching square](https://fr.wikipedia.org/wiki/Marching_squares)
* [Marching square in Unity](https://catlikecoding.com/unity/tutorials/marching-squares-series/)

## Parametres

* Zones :
  * Centre ville
    * Au centre de la carte
  * Zone industriel
    * Au abord de la ville, dans la périphérie
  * Quartier résidentiel
    * S'étend autour du centre-ville
  * Espace vert
    * Aléatoire, + grand en s'éloignant du centre ville
  * Zone d'eau
    * Dans les espaces verts
    * Fleuve
  * Zone rural
    * en dehors du rayon de la ville
  
  
plusieurs centre ville
utiliser une courbe gaussienne pour la densité des maisons/hauteur des batiments
height map
