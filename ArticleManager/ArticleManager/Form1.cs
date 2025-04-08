using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ArticleManager
{
    public partial class Form1 : Form
    {
        private readonly ArticleController controller;
        private PictureBox picArticle;
        private Button btnPrecedent, btnSuivant;
        private Label lblNumeroArticle, lblNom, lblDescription, lblMarque, lblCategorie, lblPrix;
        private TextBox txtNom, txtDescription, txtMarque, txtCategorie, txtPrix;
        private Button btnNouveau, btnModifier, btnSupprimer, btnEffacer;
        private TextBox txtFiltre;
        private CheckBox chkMontrerInvalides;
        private DataGridView dgvArticles;

        private string connectionString = @"Server=DESKTOP-243KKSF;Database=ArticleDB;Integrated Security=True;";
        private List<Article> articles;
        private int indexArticleCourant = 0;

        public Form1()
        {
            string connectionString = @"Server=DESKTOP-243KKSF;Database=ArticleDB;Integrated Security=True;";
            controller = new ArticleController(connectionString);
            InitializeComponent();
            articles = new List<Article>();
            InitializeGUI();
            ChargerArticles();
        }

        private void InitializeGUI()
        {
            this.Text = "Gestionnaire d'Articles";
            this.Size = new Size(820, 600);
            this.BackColor = Color.FromArgb(243, 244, 246);

            // Left side - Article details
            var articlePanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(450, 270),
                BackColor = Color.White
            };
            articlePanel.Paint += PaintBorderRadius;

            picArticle = new PictureBox
            {
                Size = new Size(150, 200),
                Location = new Point(10, 10),
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            btnPrecedent = new Button
            {
                Text = "<",
                Location = new Point(10, 220),
                Size = new Size(40, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White
            };
            btnPrecedent.FlatAppearance.BorderSize = 0;

            btnSuivant = new Button
            {
                Text = ">",
                Location = new Point(120, 220),
                Size = new Size(40, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White
            };
            btnSuivant.FlatAppearance.BorderSize = 0;

            // Article info labels
            lblNumeroArticle = CreateLabel("Article N° 1", 170, 10, true);
            lblNom = CreateLabel("", 170, 40);
            lblDescription = CreateLabel("", 170, 70);
            lblMarque = CreateLabel("", 170, 100);
            lblCategorie = CreateLabel("", 170, 130);
            lblPrix = CreateLabel("", 170, 160);

            // Right side - Action buttons
            var buttonsPanel = new Panel
            {
                Location = new Point(470, 10),
                Size = new Size(320, 270),
                BackColor = Color.White
            };
            buttonsPanel.Paint += PaintBorderRadius;

            btnNouveau = new Button
            {
                Text = "+ Nouveau",
                Location = new Point(10, 70),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White
            };
            btnNouveau.FlatAppearance.BorderSize = 0;

            btnModifier = new Button
            {
                Text = "✎ Modifier",
                Location = new Point(110, 70),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(79, 70, 229)
            };
            btnModifier.FlatAppearance.BorderColor = Color.FromArgb(79, 70, 229);

            btnSupprimer = new Button
            {
                Text = "✕ Supprimer",
                Location = new Point(210, 70),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White
            };
            btnSupprimer.FlatAppearance.BorderColor = Color.FromArgb(79, 70, 229);

            Label lblFiltrer = new Label
            {
                Text = "Filtrer Articles",
                Location = new Point(10, 120),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f)
            };

            txtFiltre = new TextBox
            {
                Location = new Point(10, 145),
                Size = new Size(200, 25),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnEffacer = new Button
            {
                Text = "🧹 Effacer",
                Location = new Point(220, 145),
                Size = new Size(70, 25),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(79, 70, 229)
            };
            btnEffacer.FlatAppearance.BorderColor = Color.FromArgb(79, 70, 229);

            chkMontrerInvalides = new CheckBox
            {
                Text = "Montrer articles invalides",
                Location = new Point(10, 180),
                Font = new Font("Segoe UI", 9f),
                AutoSize = true
            };

            // Bottom - DataGridView
            var gridPanel = new Panel
            {
                Location = new Point(10, 290),
                Size = new Size(780, 260),
                BackColor = Color.White
            };
            gridPanel.Paint += PaintBorderRadius;

            dgvArticles = new DataGridView
            {
                Location = new Point(10, 10),
                Size = new Size(760, 240),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(224, 224, 224)
            };

            ConfigureDataGridView();

            SetupEvents();

            articlePanel.Controls.AddRange(new Control[] {
            picArticle, btnPrecedent, btnSuivant,
            lblNumeroArticle, lblNom, lblDescription,
            lblMarque, lblCategorie, lblPrix
    });

            buttonsPanel.Controls.AddRange(new Control[] {
        btnNouveau, btnModifier, btnSupprimer,
        lblFiltrer, txtFiltre, btnEffacer, chkMontrerInvalides
    });

            gridPanel.Controls.Add(dgvArticles);

            this.Controls.AddRange(new Control[] { articlePanel, buttonsPanel, gridPanel });

            ConfigurerFiltrage();
        }

        private Label CreateLabel(string text, int x, int y, bool isTitle = false)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", isTitle ? 11f : 9.5f),
                AutoSize = true
            };
        }

        private void PaintBorderRadius(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            using (var p = new Pen(Color.FromArgb(229, 231, 235)))
            {
                e.Graphics.DrawRectangle(p, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        }

        private void ConfigureDataGridView()
        {
            dgvArticles.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgvArticles.DefaultCellStyle.SelectionBackColor = Color.FromArgb(159, 164, 242);
            dgvArticles.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvArticles.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvArticles.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgvArticles.EnableHeadersVisualStyles = false;

            dgvArticles.Columns.AddRange(new DataGridViewColumn[]
            {
            new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Code", Width = 80 },
            new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "Nom", Width = 120 },
            new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", Width = 150 },
            new DataGridViewTextBoxColumn { Name = "Brand", HeaderText = "Marque", Width = 100 },
            new DataGridViewTextBoxColumn { Name = "Category", HeaderText = "Catégorie", Width = 100 },
            new DataGridViewTextBoxColumn { Name = "Price", HeaderText = "Prix", Width = 100 }
            });
        }

        private void SetupEvents()
        {
            btnPrecedent.Click += (s, e) => { if (indexArticleCourant > 0) AfficherArticle(indexArticleCourant - 1); };
            btnSuivant.Click += (s, e) => { if (indexArticleCourant < articles.Count - 1) AfficherArticle(indexArticleCourant + 1); };
            btnNouveau.Click += (s, e) => AjouterArticle();
            btnModifier.Click += (s, e) => ModifierArticle();
            btnSupprimer.Click += (s, e) => SupprimerArticle();
        }

        private void ConfigurerFiltrage()
        {
            txtFiltre.TextChanged += FiltrerArticlesEvent;
            btnEffacer.Click += EffacerFiltresEvent;
            chkMontrerInvalides.CheckedChanged += MontrerArticlesInvalidesEvent;
        }

        private void FiltrerArticlesEvent(object sender, EventArgs e)
        {
            FiltrerArticles();
        }

        private void EffacerFiltresEvent(object sender, EventArgs e)
        {
            txtFiltre.Text = string.Empty;
            chkMontrerInvalides.Checked = false;
            ChargerArticles();
        }

        private void MontrerArticlesInvalidesEvent(object sender, EventArgs e)
        {
            FiltrerArticles();
        }

        private void ChargerArticles()
        {
            articles = controller.GetAllArticles();
            MettreAJourDataGridView(articles);
            if (articles.Count > 0) AfficherArticle(0);
        }

        private void MettreAJourDataGridView(List<Article> articlesToShow)
        {
            dgvArticles.Rows.Clear();
            foreach (var article in articlesToShow)
            {
                dgvArticles.Rows.Add(
                    article.Id,
                    article.Code,
                    article.Name,
                    article.Description,
                    article.Brand,
                    article.Category,
                    article.Price
                );
            }
        }

        private bool EstArticleValide(Article article)
        {
            return !(string.IsNullOrWhiteSpace(article.Code) ||
                    string.IsNullOrWhiteSpace(article.Name) ||
                    string.IsNullOrWhiteSpace(article.Description) ||
                    string.IsNullOrWhiteSpace(article.Brand) ||
                    string.IsNullOrWhiteSpace(article.Category) ||
                    article.Price <= 0);
        }

        private void FiltrerArticles()
        {
            var filteredArticles = controller.FilterArticles(
                txtFiltre.Text?.ToLower() ?? "",
                chkMontrerInvalides.Checked
            );
            MettreAJourDataGridView(filteredArticles);
        }

        private void AfficherArticle(int index)
        {
            if (index >= 0 && index < articles.Count)
            {
                indexArticleCourant = index;
                Article currentArticle = articles[index];

                lblNumeroArticle.Text = $"Article N° {index + 1}";
                lblNom.Text = currentArticle.Name;
                lblDescription.Text = currentArticle.Description;
                lblMarque.Text = $"Marque: {currentArticle.Brand}";
                lblCategorie.Text = $"Catégorie: {currentArticle.Category}";
                lblPrix.Text = $"Prix: {currentArticle.Price:N2}";

                if (currentArticle.Image != null)
                {
                    using (MemoryStream ms = new MemoryStream(currentArticle.Image))
                    {
                        picArticle.Image?.Dispose();
                        picArticle.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    picArticle.Image = null;
                }

                btnPrecedent.Enabled = index > 0;
                btnSuivant.Enabled = index < articles.Count - 1;
            }
        }

        private void AjouterArticle()
        {
            using (var form = new ArticleForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    controller.AddArticle(form.Article);
                    ChargerArticles();
                    AfficherArticle(articles.Count - 1);
                }
            }
        }

        private void ModifierArticle()
        {
            if (indexArticleCourant >= 0)
            {
                using (var form = new ArticleForm(articles[indexArticleCourant]))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        controller.UpdateArticle(form.Article);
                        ChargerArticles();
                        AfficherArticle(indexArticleCourant);
                    }
                }
            }
        }


        private void SupprimerArticle()
        {
            if (indexArticleCourant >= 0)
            {
                if (MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet article ?",
                    "Confirmer la suppression", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    controller.DeleteArticle(articles[indexArticleCourant].Id);
                    ChargerArticles();
                    if (articles.Count > 0)
                    {
                        AfficherArticle(Math.Max(0, indexArticleCourant - 1));
                    }
                }
            }
        }
    }
}