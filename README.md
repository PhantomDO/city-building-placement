# Création d'une ville
Ce projet permet la génération procédurale d'une ville dont les bâtiments sont des formes basiques.
 ![image]
# Source

*  [Algorithme de remplissage par diffusion](https://fr.wikipedia.org/wiki/Algorithme_de_remplissage_par_diffusion)
* [Thèse Generation procédurale de monde](https://tel.archives-ouvertes.fr/tel-00841373/document)
* [Courbe gaussiene](https://fr.wikipedia.org/wiki/Fonction_gaussienne)
* [Marching square](https://fr.wikipedia.org/wiki/Marching_squares)
* [Marching square in Unity](https://catlikecoding.com/unity/tutorials/marching-squares-series/)
* [To look](https://github.com/ProbableTrain/MapGenerator)
  
plusieurs centre ville
utiliser une courbe gaussienne pour la densité des maisons/hauteur des batiments
height map


# Content

Dans ce dépot vous trouverez le travail de recherche de placement de différents élément qui constitue une ville.
Nous avons découpé ces élements en zone et rempli notre grille par des batiments/parc/eau, etc...

Voici les paramètres des zones : 

  * Centre ville
    * Au centre de la carte
    * Radius : 0 - 0.12
  * Zone travail (entreprise)
    * Au abord de la ville, dans la périphérie
    * Radius : 0.12 - 0.20
  * Zone industriel
    * Au abord de la ville, dans la périphérie
    * Radius : 0.8 - 1.0
  * Quartier résidentiel
    * S'étend autour du centre-ville
    * Radius : 0.12 - 0.4
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


# Definition

# Architecture

# Résultats

# Conclusion
