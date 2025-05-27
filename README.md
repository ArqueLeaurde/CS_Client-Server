# CS_Client-Server
🗨️ TCP Chat Server with Console Login (C#) Un projet de chat en C# basé sur TCP, avec système d’authentification intégré côté serveur. Il permet à plusieurs clients de se connecter, s’authentifier avec identifiants valides et communiquer entre eux via des messages texte.

✨ Fonctionnalités
🔐 Authentification console :

Le serveur demande un identifiant et un mot de passe.

Seuls les utilisateurs prédéfinis dans le tableau Users[] peuvent se connecter.

📡 Communication TCP multi-clients :

Chaque client se connecte à un serveur via une socket TCP.

Un système de diffusion des messages (broadcast) permet de relayer les messages reçus à tous les autres clients connectés.

🧵 Multithreading :

Le serveur gère chaque client dans un thread dédié.

Le client utilise un thread séparé pour recevoir les messages.

💬 Interface console fluide :

Prompts de login propres.

Affichage clair des messages entrants et sortants.

🛠️ Technologies
C# .NET (Console App)

TCP/IP avec TcpClient, TcpListener

Multithreading (Thread)
