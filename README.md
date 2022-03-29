# PFE - Alimentation au service d'une alimentation saine
Thomas Heurtel - Thomas Piet

Projet de Fin d'Études réalisé dans le cadre du Master d'Informatique, parcours image et son

Université de Bordeaux 2022

Encadrants : Arnaud Prouzeau, Pascal Desbarats

En collaboration avec : Antonio Verdejo-Garcia, Simon Van Baal (université de Monash, Melbourne)

## Présentation

Le projet traite de biais cognitifs étant à l’origine de choix alimentaires malsains. Précisement
du biais d’approche, qui se traduit par une tendance à se diriger vers de la nourriture malsaine
plutôt que l’éviter. Il existe donc un entraînement cognitif pour éviter ce biais d’approche,
appelé en anglais Approach-Avoidance Training (AAT). Il consiste à entraîner les utilisateurs
à effectuer un mouvement d’évitement, dans notre cas, face à des aliments malsains et un
mouvement d’approche face à des aliments sains. 

Le but du projet est de réaliser un prototype d’application en réalité augmentée pour
smartphone, permettant la mise en oeuvre de l’AAT à domicile sur une plateforme familière
et ainsi tenter de rendre l’utilisation de cette entraînement cognitif encore plus réaliste et
attrayant pour permettre une utilisation par la population globale en recherche de solution
pour une alimentation saine. L’application doit permettre aux utilisateurs de visualiser des
aliments virtuels sur une surface plane (table de diner, plan de travail dans une cuisine, ..) et
leur permettre de les jeter ou non en applicant l’AAT à l’aide de gestes physique (mouvement
du téléphone ou de la main).

Développé sous Unity en C# avec le toolkit ARFoundation pour une utilisation sous Android.

## Installation
apk proposé directement, sinon :

### Prérequis

Unity (projet réalisé sur la version 2020.3.26f1)

Smartphone Android 7.0 ou +

### Éditer le projet avec Unity

Télécharger le projet et l'ouvrir sur Unity ("Add project from disk" depuis UnityHub ou directement dans le menu File) à la racine.

### Build de l'apk et installation

Dans Unity : File -> Build Settings -> Android. La première fois, cliquer sur "Switch Platform". 

Optionnel : brancher son smartphone et le sélectionner dans "Run Device" pour une meilleur compatibilité.

Vérifier que les scènes sont bien chargées. Sinon, les glisser-déposer dans le rectangle "Scenes In Buid" en mettant la scène Menu à l'index 0.

Puis cliquer sur Build et déplacer le fichier apk obtenu sur le téléphone. L'ouvrir pour installer l'application.

## Utilisation

Le premier écran, le menu principal, s’affiche au lancement de l’application. Il permet de
lancer l'écran de jeu principal ou d’accéder aux autres menus. Tous les autres écrans ont un bouton
en haut à gauche qui permet de retourner au menu principal.

L’écran de jeu principal propose d’utiliser notre implémentation de l’Approach-Avoidance
Training en réalité augmentée. Il commence par détecter les plans sur lesquels on pourra ensuite
poser un aliment aléatoire par une pression du doigt. Une fois l’aliment posé, on peut le déplacer
sur le plan. En pressant le bouton en bas de l’écran, on va pouvoir « l’attraper », puis l’approcher
ou l’éloigner le long l’axe établi entre sa position de départ et la position de départ du téléphone
(plus précisément sa caméra). Si on lâche le bouton, l’aliment retourne à sa position de départ.
Sinon, une fois qu’il a parcouru une certaine distance ; il va valider notre choix et effectuer des
actions en conséquence. Si on l’a rapproché de soi pour le « manger », l’objet va continuer de se
rapprocher jusqu’à la caméra (en suivant cette fois l’axe entre sa position lors de la validation
et la position de la caméra au même moment). Si on l’a jeté au loin, il va s’éloigner de la caméra
et disparaître au bout de quelques frames. Puis un visage souriant ou en colère s’affiche selon
notre choix. Au bout de deux secondes, le visage disparaît et un nouvel aliment apparaît là
où le précédent se trouvait sur le plan (si on touche le plan avant la disparition de l’image,
l’aliment s’y place immédiatement).

/!\ Il est recommandé d'approcher le téléphone de l'objet avant d'appuyer sur le bouton, la distance à parcourir sera moins grande et l'utilisateur ne risque pas de devoir reculer. Cela permet de mieux simuler le fait d'attraper l'objet : on approche sa main, une fois qu'il est à portée on appuie sur le bouton pour l'attraper, puis on le ramène pour le manger ou on le jette au loin.

Au niveau de l’interface, on a un bouton en bas à gauche qui permet d’activer ou de désac-
tiver l’affichage des plans, et trois compteurs se trouvent en haut à droite de l’écran : un pour le
score qui s’incrémente ou se décrémente de dix à chaque choix, un pour l’argent, qui augmente
de dix pour chaque bon choix, et un pour la chaîne de succès actuelle. Une fois par jour, comme
récompense de connexion journalière, réaliser une chaîne de dix permet de gagner mille golds
d’un coup. L’argent et le score sont sauvegardés, mais la chaîne se réinitialise en retournant au
menu ou en quittant l’application.

L’écran de skins permet à l’utilisateur de changer l’apparence du bouton. L’apparence par
défaut est gratuite, puis il faut payer avec l’argent gagné pour obtenir les autres. Une fois ache-
tées, elles sont débloquées définitivement et au lieu d’afficher leur prix, leurs boutons respectifs
proposent de les équiper directement.

L’écran d’options permet deux choses. On peut y réinitialiser les données sauvegardées
(score, quantité de golds, skins débloqués). Pour l’activer il faut maintenir le bouton reset en-
foncé pendant cinq secondes. En choisissant l’option swipe, on bascule sur un nouvel écran qui
permet d’essayer la première version de l’application : il suffit de swiper sur l’écran au lieu de
déplacer son téléphone. Cette version ayant été rajoutée après coup en bonus, elle n’est pas liée
à toutes les autres fonctionnalités et ne permet pas de changer son score, ses golds ou son skin.

