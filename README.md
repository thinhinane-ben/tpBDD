# 📊 Projet tpBDD — Gestionnaire d'Articles (MVC)

Ce projet est une application de gestion d'articles développée en C# avec Windows Forms, suivant le **pattern MVC** (Modèle-Vue-Contrôleur) et le pattern **Repository**.

---

## 🧱 Architecture du Projet

### 🔹 Modèle
- `Article.cs` : Classe POCO représentant les données des articles.

### 🔹 Contrôleur
- `ArticleController.cs` : Gère la logique métier, les interactions avec le modèle et la vue, et les opérations CRUD via le repository.

### 🔹 Vues
- `Form1.cs` : Vue principale affichant la liste des articles avec filtres.
- `ArticleForm.cs` : Formulaire d’édition d’un article (ajout, modification, suppression).

### 🔹 Repository
- `ArticleRepository.cs` : Gère les interactions avec la base de données (recherche, ajout, mise à jour...).

---

## 🔁 Fonctionnalités

- Ajout, modification, suppression d'articles
- Filtrage dynamique via l’interface utilisateur
- Connexion à une base de données via `ArticleRepository`
- Architecture découplée pour faciliter les tests et l’évolution

---

## 🗂️ Structure MVC + Repository

```plaintext
[Vue] Form1.cs / ArticleForm.cs
    ⇅
[Contrôleur] ArticleController.cs
    ⇅
[Repository] ArticleRepository.cs
    ⇅
[Base de Données]



