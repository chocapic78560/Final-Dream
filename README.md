# Final Dream 🌙

**Jeu vidéo de plateforme 2D onirique**, développé sous **Unity (C#)** par l'équipe **BLOAM CORPORATION**, dans le cadre d'un projet de développement de jeu vidéo.

![Genre](https://img.shields.io/badge/genre-plateforme%202D-blueviolet)
![Moteur](https://img.shields.io/badge/moteur-Unity-black)
![Langage](https://img.shields.io/badge/langage-C%23-239120)
![Multijoueur](https://img.shields.io/badge/multijoueur-LAN-informational)
![Statut](https://img.shields.io/badge/statut-projet%20académique-yellow)

---

## 📖 Sommaire

- [Présentation](#-présentation)
- [Histoire et univers](#-histoire-et-univers)
- [Fonctionnalités](#-fonctionnalités)
- [Intelligence artificielle](#-intelligence-artificielle)
- [Mode multijoueur (LAN)](#-mode-multijoueur-lan)
- [Système de santé](#-système-de-santé)
- [Level design et environnement](#-level-design-et-environnement)
- [Personnages et animation](#-personnages-et-animation)
- [Ennemis](#-ennemis)
- [Bande sonore](#-bande-sonore)
- [Site web du projet](#-site-web-du-projet)
- [Stack technique](#-stack-technique)
- [Structure du dépôt](#-structure-du-dépôt)
- [Installation et lancement](#-installation-et-lancement)
- [Difficultés rencontrées](#-difficultés-rencontrées-et-solutions)
- [Limites et pistes d'amélioration](#-limites-et-pistes-damélioration)
- [Équipe](#-équipe--bloam-corporation)
- [Sources et références](#-sources-et-références)
- [Licence](#-licence)

---

## 🎮 Présentation

**Final Dream** est un jeu de plateforme en 2D dans lequel chaque niveau représente un rêve distinct, entre paysages féeriques, environnements symboliques et scénarios plus sombres. Le projet a été conçu comme une synthèse entre compétences techniques et vision artistique : gameplay de plateforme précis, IA d'ennemis basée sur l'algorithme A*, mode coopératif en réseau local, identité visuelle et sonore cohérente.

Le développement s'est appuyé sur des références marquantes du genre plateforme — *Hollow Knight*, *Inside*, *Dead Cells*, *Super Mario Galaxy*, *Donkey Kong Country Returns* — tout en cherchant à proposer une direction artistique et narrative originale autour du thème du rêve.

Le projet a été structuré en quatre phases :
1. **Pré-production** : conceptualisation du scénario, définition des mécaniques, choix des outils (Unity, GarageBand, Procreate)
2. **Production** : développement des niveaux, intégration des assets graphiques et sonores, programmation de l'IA et du réseau
3. **Tests** : validation des fonctionnalités et optimisation des performances
4. **Publication** : site web du projet et préparation de la soutenance finale

---

## 📜 Histoire et univers

L'histoire débute lorsque le frère cadet du protagoniste, inquiet de le voir plongé dans un sommeil profond, utilise une machine expérimentale pour entrer dans ses rêves. Ce choix narratif introduit naturellement le mode coopératif : les deux joueurs incarnent respectivement **Milo**, explorateur/aventurier au caractère haut en couleur, et **Elena**, sa compagne d'aventure en mode multijoueur.

Chaque niveau est un rêve à part entière, avec son propre objectif et sa propre logique — reflétant la nature fragmentée et parfois incohérente des rêves. Le scénario s'articule en quatre actes autour des thèmes du regret, de la rédemption et de l'acceptation de soi, en s'inspirant de la narration minimaliste de *Hollow Knight*, de l'ambiance symbolique d'*Inside* et de l'usage d'objets du quotidien comme vecteurs émotionnels dans *Unravel*.

---

## ✨ Fonctionnalités

- **Déplacements et contrôle du personnage** : course (seul mode de déplacement horizontal), saut avec détection d'état (sol / montée / chute), accroupissement
- **Système d'attaque dynamique** : activation par un seul bouton, sélection aléatoire entre deux animations d'attaque distinctes, détection de zone d'impact, cooldown anti-spam
- **Mode multijoueur en LAN** coopératif, jusqu'à deux joueurs
- **IA ennemie réactive** basée sur l'algorithme A*, avec détection et poursuite du joueur le plus proche
- **Zones environnementales interactives** : dégâts progressifs (eau), mort instantanée (lave, chute dans le vide)
- **Système de téléportation** entre zones de niveau, basé sur colliders et tags
- **Barre de vie dynamique** (joueur et ennemis), avec dégradé de couleur selon le niveau de santé
- **Deux personnages jouables** avec animations distinctes, dessinées en pixel art
- **Deux archétypes d'ennemis** : un aérien rapide et fragile, un terrestre robuste
- **Bande sonore originale**, différente pour chaque niveau et pour le menu
- **Menu principal thématique**, autour de l'imagerie du cerveau et de l'onirisme

---

## 🧠 Intelligence artificielle

L'IA des ennemis repose sur l'algorithme de pathfinding **A\*** (A-Star), qui combine :
- l'algorithme de **Dijkstra** (garantit le chemin le plus court),
- une recherche **best-first** guidée par une heuristique.

La fonction de coût utilisée est :

```
f(n) = g(n) + h(n)
```

où `g(n)` est le coût réel du chemin depuis le départ jusqu'au nœud `n`, et `h(n)` l'estimation heuristique du coût restant jusqu'à la destination. L'heuristique retenue est la **distance de Manhattan**, adaptée aux déplacements sur grille :

```
h(n) = |x_destination − x_n| + |y_destination − y_n|
```

**Implémentation :**
- Discrétisation de l'environnement en grille de cellules traversables / non traversables
- Gestion d'une liste ouverte et d'une liste fermée de nœuds
- Reconstruction du chemin une fois le nœud de destination atteint

**Optimisations mises en place :**
- Limitation de la profondeur de recherche
- Mise en cache des chemins récemment calculés
- Recalcul du chemin uniquement en cas de changement significatif (position de la cible, obstacle nouveau)

**Détection et priorisation de cible :**
Chaque ennemi maintient une liste des joueurs présents dans la scène, mise à jour via un système d'événements Unity (`OnPlayerSpawned`) plutôt que par recherche répétée d'objets tagués — ce choix a résolu un bug de détection lié à l'instanciation des joueurs via Mirror. En contexte multijoueur, l'ennemi cible systématiquement **le joueur le plus proche**, recalculé à chaque renouvellement de chemin.

---

## 🌐 Mode multijoueur (LAN)

Le jeu implémente un mode coopératif à deux joueurs, exclusivement en **réseau local (LAN)**, pour les raisons suivantes :
- **Latence réduite** : connexions directes entre appareils, sans passer par un serveur distant
- **Simplicité d'implémentation** : pas de gestion d'hébergement en ligne
- **Sécurité accrue** : surface d'attaque réduite par rapport à une connexion Internet
- **Expérience sociale** : favorise le jeu en présentiel

**Architecture :**
- Modèle **client-serveur** simplifié : un joueur hôte (serveur), un joueur client
- **Synchronisation** : envoi périodique de l'état complet du jeu par le serveur, validation des actions du client côté serveur, prédiction côté client pour limiter la perception de latence
- Fonctions **`ClientRpc`** et **`Command`** (Mirror) pour synchroniser positions et animations entre les deux clients

**Connexion :**
1. L'hôte lance le jeu et sélectionne **Play** dans le menu principal
2. L'hôte communique son adresse IP locale au client
3. Le client sélectionne **Join Party** et saisit l'adresse IP de l'hôte
4. Une fois la connexion établie, les deux joueurs sont redirigés vers l'écran de jeu

---

## ❤️ Système de santé

Le système de santé repose sur deux classes principales, gérant la logique et sa représentation visuelle :
- Gestion des points de vie maximum et courants
- Prise de dégâts avec effets visuels associés
- Fonction de soin avec mise à jour de l'interface
- Système de mort et de réapparition automatique (désactivation temporaire des contrôles, animation de mort, respawn au dernier point de sauvegarde)

**Interface** : barre de vie construite avec un `Slider` UI (Background / Fill Area / Handle), changeant de couleur selon le niveau de santé (vert → jaune → rouge), avec transitions fluides.

**Zones de danger environnementales :**
- **Eau nocive** : dégâts continus (≈ 5 PV/seconde)
- **Lave** : mort instantanée au contact, avec respawn
- **Chute dans le vide** : mort instantanée, ajoutant une dimension stratégique aux déplacements

---

## 🗺️ Level design et environnement

- **Menu principal** : design original en forme de cerveau, généré assisté par IA puis intégré et retravaillé dans Unity, avec effet d'éclairage au survol des boutons
- **Palette de tiles** : palettes personnalisées via l'éditeur Unity, accélérant la création des environnements et garantissant leur homogénéité graphique
- **Tilesets** : sélection rigoureuse selon la cohérence esthétique, la compatibilité de résolution et la modularité (variations climatiques, transitions de terrain, éléments de détail)
- **Téléportation** : `BoxCollider2D` positionnés stratégiquement + système de tags, permettant un code réutilisable et une intégration rapide de nouveaux points de téléportation
- **Prefabs de portes téléporteurs** : sprite, collider et script de téléportation encapsulés, garantissant cohérence visuelle et maintenabilité
- **Backgrounds** : images de fond intégrées via un Canvas en mode *World Space*, pour la profondeur visuelle sans coût de performance significatif
- **Objets immersifs** : éléments naturels, décor urbain, accessoires environnementaux, organisés en prefabs réutilisables

---

## 🧍 Personnages et animation

Le travail d'animation s'est déroulé en plusieurs étapes :
1. **Personnage générique** : toutes les animations de base (idle, course, saut, atterrissage, accroupissement, attaques) ont été prototypées sur un modèle blanc simplifié
2. **Milo** (personnage solo) : design d'aventurier, dessiné en pixel art image par image sous **Procreate**, puis exporté/réimporté dans Unity pour chaque frame d'animation
3. **Elena** (deuxième personnage, mode coopératif) : même pipeline de production, avec un design visuel distinct pour différencier clairement les deux héros à l'écran

**Structure de l'Animator Controller du joueur :**
- État de base (Idle)
- État de déplacement
- État de saut (décollage / air / atterrissage)
- États d'attaque (2 variations)
- État de mort et réapparition

---

## 👾 Ennemis

Deux archétypes ont été retenus depuis l'Unity Asset Store, adaptés à la direction artistique du jeu :
- **Chauve-souris volante** : ennemi aérien, rapide mais fragile, capable de poursuivre le joueur en 2D
- **Gobelin terrestre** : adversaire plus robuste, menace de combat rapproché

**Système d'animation** : `Animator Controller` dédié par type d'ennemi, avec trois états principaux (Idle, Attaque, Mort).

**Système de santé des ennemis** : classe dédiée gérant les PV selon le type, avec séquence de mort complète (animation, désactivation des colliders/scripts, destruction différée de l'objet).

---

## 🎵 Bande sonore

Composée intégralement avec **GarageBand**, la bande sonore accompagne la progression émotionnelle du jeu :

| Niveau | Ambiance | Éléments utilisés |
|---|---|---|
| Menu | Onirique, immersive | Pad "Final Twilight" |
| Niveau 1 | Dynamique, énergique | Drummer "Deep Tech" + Electronic Drum Kit "Deep Sub Bass" |
| Niveau 2 | Intense, rapide (160 BPM) | Drummer "After Party" + Kits "Hard Knock" et "Beat Machine" |
| Niveau final | Apaisante, sereine (75 BPM) | Drummer "Slow Jam" + Electronic Drum Kit "Boutique 78" |

---

## 🌍 Site web du projet

Un site web dédié a été développé en parallèle pour présenter le projet et suivre son évolution.

**Stack :** Vue.js, HTML/CSS/JavaScript, Tailwind CSS.

**Inspirations :** design épuré du site Anthropic, documentation W3Schools, tutoriels vidéo pour la prise en main de Vue.js.

---

## 🛠️ Stack technique

| Domaine | Outils |
|---|---|
| Moteur de jeu | Unity (2D) |
| Langage | C# |
| Réseau (LAN) | Mirror (client-serveur, `ClientRpc` / `Command`) |
| IA | Algorithme A* (pathfinding maison) |
| Level design | Tile Palette Unity, tilesets, Canvas *World Space* |
| Animation personnages | Procreate (pixel art, frame par frame) |
| Audio | GarageBand |
| Site web du projet | Vue.js, Tailwind CSS, HTML/CSS/JS |

---

## 📁 Structure du dépôt

```
Final-Dream/
├── Assets/              # Assets Unity : scripts, sprites, animations, sons, scènes, prefabs
├── Packages/            # Dépendances Unity (Package Manager)
├── ProjectSettings/     # Configuration du projet Unity
├── Final-Dream_Data/    # Données du build (exécutable Windows)
├── Final-Dream.exe      # Build jouable du jeu
└── README.md
```

---

## 🚀 Installation et lancement

### Option 1 — Jouer directement (build Windows)
Lancer `Final-Dream.exe` à la racine du dépôt (nécessite les fichiers `Final-Dream_Data`, `MonoBleedingEdge` et `UnityPlayer.dll` présents à côté de l'exécutable).

### Option 2 — Ouvrir le projet dans Unity
```bash
git clone https://github.com/chocapic78560/Final-Dream.git
```
Ouvrir ensuite le dossier via **Unity Hub**, sélectionner la version Unity correspondante, puis ouvrir la scène du menu principal.

### Mode multijoueur (LAN)
1. Sur le poste hôte : lancer le jeu, sélectionner **Play** dans le menu principal
2. Relever l'adresse IP locale affichée
3. Sur le poste client : lancer le jeu, sélectionner **Join Party**, saisir l'adresse IP de l'hôte
4. Une fois connectés, la partie coopérative démarre pour les deux joueurs

---

## 🧩 Difficultés rencontrées et solutions

| Problème | Cause | Solution |
|---|---|---|
| Ennemis oscillant entre cibles / ignorant un joueur | Mauvaise détection des tags à l'instanciation des joueurs via Mirror | Système d'événement `OnPlayerSpawned`, auquel les ennemis s'abonnent |
| Pathfinding défaillant sur grande grille | Erreur de conversion des coordonnées nœud ↔ monde | Correction du calcul de conversion de coordonnées |
| Désynchronisation des positions/animations en réseau | Absence de mécanisme de synchronisation robuste | Mise en place de fonctions `ClientRpc` et `Command` |
| Boutons du menu trop petits | Taille non adaptée aux grands écrans | Ajustement manuel de la taille, tests sur plusieurs résolutions |

---

## 🔭 Limites et pistes d'amélioration

- **Interface utilisateur** : pourrait être enrichie d'animations et d'effets visuels supplémentaires
- **Équilibrage du gameplay** : certains niveaux gagneraient à une courbe de difficulté plus progressive
- **Accroupissement** : le joueur peut actuellement se déplacer normalement tout en étant accroupi (correction prévue)
- **Extension du multijoueur** : passage d'un mode LAN à un mode en ligne pour élargir l'audience

---

## 👥 Équipe — BLOAM CORPORATION

| Membre | Contribution principale |
|---|---|
| **Benjamin Poussart** | Partie réseau (LAN) et intelligence artificielle (A*) |
| **Mattéo Galus** | Création des maps (palettes, tilesets) |
| **Alexandre Abbuzzese** | Gestion des ennemis, barre de vie, animations joueur, site web |
| **Oren Guetta** | Menu principal, musiques, animations des personnages |
| **Léon Monéger** | Support transverse sur les tâches de l'équipe |

---

## 📚 Sources et références

**Scénario**
- [GameDeveloper.com — Narrative Design in Games](https://www.gamedeveloper.com)
- Le voyage du héros — objectif-scenario.fr
- JeuxVideo.com — forum "Comment écrire un scénario de jeu vidéo"

**Réseau**
- [Documentation Unity](https://docs.unity.com/)
- [Unity Netcode for GameObjects](https://docs-multiplayer.unity3d.com/netcode/current/tutorials/get-started-ngo/)
- SharpCoderBlog — Building multiplayer networked games in Unity

**Programmation solo**
- Stack Overflow — scripts de saut Unity 2D
- Tutoriels YouTube — Design Patterns

**Intelligence artificielle**
- [Algorithme A* — Wikipédia](https://fr.wikipedia.org/wiki/Algorithme_A*)

---

## 📄 Licence

Projet académique réalisé dans le cadre d'un projet de développement de jeu vidéo en groupe (première année). Tous droits réservés à l'équipe BLOAM CORPORATION sauf mention contraire pour les assets tiers utilisés (Unity Asset Store).
