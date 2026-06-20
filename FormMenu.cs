using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GAMECARO7X7
{
    // ========================================
    public class ClassicButton : Button
    {
        //Khai báo 
        private bool _isHovered = false;
        private bool _isPressed = false;
        private Color _baseColor;
        private Color _accentColor;

        public Color BaseColor { get => _baseColor; set { _baseColor = value; Invalidate(); } }
        public Color AccentColor { get => _accentColor; set { _accentColor = value; Invalidate(); } }

        public ClassicButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(255, 248, 220); // Cornsilk
            Font = new Font("Georgia", 13, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Size = new Size(240, 50);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; _isPressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int radius = 8;
            Rectangle outerRect = new Rectangle(0, 0, Width - 1, Height - 1);
            Rectangle innerRect = new Rectangle(3, 3, Width - 7, Height - 7);

            // --- Bóng đổ ---
            if (!_isPressed)
            {
                Rectangle shadowRect = new Rectangle(3, 4, Width - 1, Height - 1);
                using (GraphicsPath shadowPath = RoundRect(shadowRect, radius))
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(80, 0, 0, 0)))
                {
                    g.FillPath(shadowBrush, shadowPath);
                }
            }

            // --- Viền ngoài (beveled edge) ---
            Color borderLight = _isPressed ? Darken(_baseColor, 20) : Lighten(_baseColor, 60);
            Color borderDark = _isPressed ? Lighten(_baseColor, 20) : Darken(_baseColor, 60);

            using (GraphicsPath outerPath = RoundRect(outerRect, radius))
            using (LinearGradientBrush borderBrush = new LinearGradientBrush(outerRect, borderLight, borderDark, 135f))
            {
                g.FillPath(borderBrush, outerPath);
            }

            // --- Nền chính ---
            Color topColor, botColor;
            if (_isPressed)
            {
                topColor = Darken(_baseColor, 30);
                botColor = Darken(_baseColor, 50);
            }
            else if (_isHovered)
            {
                topColor = Lighten(_baseColor, 25);
                botColor = _baseColor;
            }
            else
            {
                topColor = Lighten(_baseColor, 10);
                botColor = Darken(_baseColor, 20);
            }

            using (GraphicsPath innerPath = RoundRect(innerRect, radius - 2))
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(innerRect, topColor, botColor, 90f))
            {
                g.FillPath(bgBrush, innerPath);
            }

            //trang trí trông giống gỗ
            using (Pen woodPen = new Pen(Color.FromArgb(18, 0, 0, 0), 1f))
            {
                for (int y = innerRect.Y + 6; y < innerRect.Bottom - 3; y += 5)
                {
                    g.DrawLine(woodPen, innerRect.X + 8, y, innerRect.Right - 8, y);
                }
            }

            //Viền vàng trang trí
            if (_isHovered)
            {
                using (GraphicsPath glowPath = RoundRect(innerRect, radius - 2))
                using (Pen goldPen = new Pen(Color.FromArgb(120, 218, 165, 32), 1.5f)) // GoldenRod
                {
                    g.DrawPath(goldPen, glowPath);
                }
            }

            //Highlight sáng trên cùng
            Rectangle hlRect = new Rectangle(innerRect.X + 2, innerRect.Y + 1, innerRect.Width - 4, innerRect.Height / 3);
            using (GraphicsPath hlPath = RoundRect(hlRect, radius - 3))
            using (LinearGradientBrush hlBrush = new LinearGradientBrush(hlRect,
                Color.FromArgb(45, 255, 255, 255), Color.FromArgb(0, 255, 255, 255), 90f))
            {
                g.FillPath(hlBrush, hlPath);
            }

            //Text với bóng
            Rectangle textRect = _isPressed ? new Rectangle(1, 1, Width - 1, Height - 1) : new Rectangle(0, 0, Width - 1, Height - 1);

            //Bóng chữ
            Rectangle shadowTextRect = new Rectangle(textRect.X + 1, textRect.Y + 1, textRect.Width, textRect.Height);
            TextRenderer.DrawText(g, Text, Font, shadowTextRect, Color.FromArgb(100, 0, 0, 0),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // Chữ chính
            TextRenderer.DrawText(g, Text, Font, textRect, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private GraphicsPath RoundRect(Rectangle b, int r)
        {
            int d = r * 2;
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(b.X, b.Y, d, d, 180, 90);
            gp.AddArc(b.Right - d, b.Y, d, d, 270, 90);
            gp.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90);
            gp.AddArc(b.X, b.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private Color Lighten(Color c, int a) => Color.FromArgb(c.A, Math.Min(255, c.R + a), Math.Min(255, c.G + a), Math.Min(255, c.B + a));
        private Color Darken(Color c, int a) => Color.FromArgb(c.A, Math.Max(0, c.R - a), Math.Max(0, c.G - a), Math.Max(0, c.B - a));
    }

    // ==================== PANEL KHUNG GỖ ====================
    public class ClassicPanel : Panel
    {
        private int _radius = 10;
        private Color _fillColor = Color.FromArgb(60, 245, 222, 179); // Wheat translucent
        private Color _borderColor = Color.FromArgb(139, 90, 43); // SaddleBrown

        public int Radius { get => _radius; set { _radius = value; Invalidate(); } }
        public Color FillColor { get => _fillColor; set { _fillColor = value; Invalidate(); } }
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        public ClassicPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            Rectangle innerRect = new Rectangle(3, 3, Width - 7, Height - 7);

            using (GraphicsPath path = RoundRect(rect, _radius))
            {
                // Nền trông ấm
                using (LinearGradientBrush bgBrush = new LinearGradientBrush(rect,
                    Color.FromArgb(220, 253, 245, 230), // OldLace
                    Color.FromArgb(220, 245, 222, 179), // Wheat
                    135f))
                {
                    g.FillPath(bgBrush, path);
                }

                // Viền giống gỗ kép
                using (Pen outerPen = new Pen(Color.FromArgb(180, 101, 67, 33), 3f)) // DarkBrown
                    g.DrawPath(outerPen, path);

                using (GraphicsPath innerPath = RoundRect(innerRect, _radius - 2))
                using (Pen innerPen = new Pen(Color.FromArgb(120, 160, 120, 60), 1f))
                    g.DrawPath(innerPen, innerPath);
            }

            // Trang trí góc nhỏ
            DrawCornerOrnament(g, 8, 8);
            DrawCornerOrnament(g, Width - 16, 8);
            DrawCornerOrnament(g, 8, Height - 16);
            DrawCornerOrnament(g, Width - 16, Height - 16);

            base.OnPaint(e);
        }

        private void DrawCornerOrnament(Graphics g, int x, int y)
        {
            using (Pen goldPen = new Pen(Color.FromArgb(140, 184, 134, 11), 1.5f)) // DarkGoldenRod
            {
                g.DrawRectangle(goldPen, x, y, 6, 6);
                g.DrawLine(goldPen, x + 3, y, x + 3, y + 6);
                g.DrawLine(goldPen, x, y + 3, x + 6, y + 3);
            }
        }

        private GraphicsPath RoundRect(Rectangle b, int r)
        {
            int d = r * 2;
            GraphicsPath gp = new GraphicsPath();
            gp.AddArc(b.X, b.Y, d, d, 180, 90);
            gp.AddArc(b.Right - d, b.Y, d, d, 270, 90);
            gp.AddArc(b.Right - d, b.Bottom - d, d, d, 0, 90);
            gp.AddArc(b.X, b.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }
    }

    // ==================== FORM MENU CHÍNH====================
    public partial class FormMenu : Form
    {
        //Controls
        private Button btnChoiNgay, btnCaiDat, btnLuatChoi, btnThoat;
        private ClassicButton btnTiepTuc; // Nút tiếp tục chơi
        private ClassicPanel pnlCaiDat;
        private ComboBox cbxDoKho, cbxKichThuoc;
        private TrackBar trbAmLuong;
        private Label lblAmLuongValue;
        private Label lblTieuDe, lblPhuDe;

        // Tham chiếu đến game đang chơi (để tiếp tục / chơi mới)
        private Form1 _currentGame;

        //Timer cho hiệu ứng
        private Timer _sparkleTimer;
        private float _sparkleAngle = 0;

        //Màu sắc chủ đạo cổ điển
        private static readonly Color WOOD_DARK = Color.FromArgb(101, 67, 33);       // Gỗ đậm
        private static readonly Color WOOD_LIGHT = Color.FromArgb(181, 137, 80);     // Gỗ sáng
        private static readonly Color GOLD = Color.FromArgb(218, 165, 32);           // Vàng gold
        private static readonly Color CREAM = Color.FromArgb(255, 253, 245);         // Kem nhạt
        private static readonly Color INK_DARK = Color.FromArgb(45, 30, 15);         // Mực đen nâu
        private static readonly Color INK_BROWN = Color.FromArgb(100, 70, 40);       // Mực nâu

        // Màu nút
        private static readonly Color BTN_GREEN = Color.FromArgb(76, 110, 60);       // Xanh rêu
        private static readonly Color BTN_BLUE = Color.FromArgb(65, 90, 120);        // Xanh cổ điển
        private static readonly Color BTN_ORANGE = Color.FromArgb(160, 100, 40);     // Cam nâu
        private static readonly Color BTN_RED = Color.FromArgb(140, 55, 45);         // Đỏ gạch

        public FormMenu()
        {
            InitializeComponent();
            KhoiTaoUI();

            // Gắn sự kiện kéo thanh âm lượng
            trbAmLuong.Scroll += (s, e) =>
            {
                C_AmThanh.SetAmLuong(trbAmLuong.Value);
                lblAmLuongValue.Text = trbAmLuong.Value + "%";
            };

            // Set âm lượng ban đầu và bật nhạc nền
            C_AmThanh.SetAmLuong(trbAmLuong.Value);
            C_AmThanh.PhatNhacNen();

            // Timer cho hiệu ứng viền
            _sparkleTimer = new Timer { Interval = 50 };
            _sparkleTimer.Tick += (s, e) => { _sparkleAngle += 2f; if (_sparkleAngle >= 360) _sparkleAngle = 0; Invalidate(); };
            _sparkleTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = ClientSize.Width;
            int h = ClientSize.Height;

            //Nền gradient sáng (WhiteSmoke)
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(248, 248, 248),   // WhiteSmoke sáng trên
                Color.FromArgb(230, 230, 235),   // Xám nhạt dưới
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            //Hiệu ứng vân giấy cũ
            Random rng = new Random(42); // Seed cố định để không nhấp nháy
            using (Pen grainPen = new Pen(Color.FromArgb(10, 100, 100, 110), 0.5f))
            {
                for (int i = 0; i < 120; i++)
                {
                    int x1 = rng.Next(w);
                    int y1 = rng.Next(h);
                    g.DrawLine(grainPen, x1, y1, x1 + rng.Next(10, 40), y1 + rng.Next(-2, 3));
                }
            }

            //Khung viền gỗ bao quanh
            int border = 12;
            Rectangle frameOuter = new Rectangle(border, border, w - border * 2, h - border * 2);
            Rectangle frameInner = new Rectangle(border + 5, border + 5, w - border * 2 - 10, h - border * 2 - 10);

            // Viền gỗ đậm
            using (Pen framePen = new Pen(WOOD_DARK, 4f))
                g.DrawRectangle(framePen, frameOuter);

            // Viền gỗ sáng (inner)
            using (Pen innerPen = new Pen(Color.FromArgb(180, WOOD_LIGHT), 1.5f))
                g.DrawRectangle(innerPen, frameInner);

            //Trang trí góc cổ điển
            DrawOrnateCorner(g, border, border);
            DrawOrnateCorner(g, w - border - 24, border);
            DrawOrnateCorner(g, border, h - border - 24);
            DrawOrnateCorner(g, w - border - 24, h - border - 24);

            // Đường kẻ trang trí dưới tiêu đề
            int lineY = 110;
            using (Pen linePen = new Pen(Color.FromArgb(140, GOLD), 1.5f))
            {
                // Đường chính
                g.DrawLine(linePen, 60, lineY, w - 60, lineY);

                // Hoa văn giữa 
                int cx = w / 2;
                g.DrawEllipse(linePen, cx - 4, lineY - 4, 8, 8);
                g.FillEllipse(new SolidBrush(Color.FromArgb(140, GOLD)), cx - 2, lineY - 2, 4, 4);

                // Hoa văn 2 bên
                g.DrawLine(linePen, cx - 30, lineY - 3, cx - 15, lineY);
                g.DrawLine(linePen, cx - 30, lineY + 3, cx - 15, lineY);
                g.DrawLine(linePen, cx + 15, lineY, cx + 30, lineY - 3);
                g.DrawLine(linePen, cx + 15, lineY, cx + 30, lineY + 3);
            }

            //Viền vàng trang trí chạy
            float sparkleX = border + 10 + (_sparkleAngle / 360f) * (w - border * 2 - 20);
            using (SolidBrush sparkleBrush = new SolidBrush(Color.FromArgb(80, GOLD)))
            {
                g.FillEllipse(sparkleBrush, sparkleX - 8, border + 2, 16, 6);
                g.FillEllipse(sparkleBrush, sparkleX - 8, h - border - 8, 16, 6);
            }

            //Họa tiết quân cờ mờ ở góc
            using (Font chessFont = new Font("Georgia", 40, FontStyle.Bold))
            {
                using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(20, INK_DARK)))
                {
                    g.DrawString("✕", chessFont, fadeBrush, 30, h - 90);
                    g.DrawString("○", chessFont, fadeBrush, w - 80, h - 90);
                }
            }
        }

        private void DrawOrnateCorner(Graphics g, int x, int y)
        {
            using (Pen goldPen = new Pen(Color.FromArgb(200, GOLD), 2f))
            {
                // Hoa văn góc
                g.DrawArc(goldPen, x, y, 20, 20, 180, 90);
                g.DrawLine(goldPen, x + 10, y, x + 22, y);
                g.DrawLine(goldPen, x, y + 10, x, y + 22);

                // Chấm trang trí
                using (SolidBrush dotBrush = new SolidBrush(Color.FromArgb(160, GOLD)))
                {
                    g.FillEllipse(dotBrush, x + 8, y + 8, 5, 5);
                }
            }
        }

        private void KhoiTaoUI()
        {
            this.Text = "Cờ Caro - Menu Chính";
            this.ClientSize = new Size(520, 710);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;
            this.Icon = Properties.Resources.iconapp;

            //Tiêu đề
            lblTieuDe = new Label
            {
                Text = "CỜ CARO",
                Font = new Font("Georgia", 32, FontStyle.Bold | FontStyle.Italic),
                ForeColor = INK_DARK,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblTieuDe.Location = new Point((520 - lblTieuDe.PreferredWidth) / 2, 30);

            lblPhuDe = new Label
            {
                Text = "—  Nhóm 3  ·  Chủ đề 6  —",
                Font = new Font("Georgia", 10, FontStyle.Italic),
                ForeColor = INK_BROWN,
                AutoSize = true,
                BackColor = Color.Transparent
            };
            lblPhuDe.Location = new Point((520 - lblPhuDe.PreferredWidth) / 2, 80);

            //Nút bấm
            int btnX = 140;
            int btnY = 130;
            int gap = 62;

            btnChoiNgay = TaoNutCoXua("🎮 Chơi Ngay", new Point(btnX, btnY), BTN_GREEN);
            btnChoiNgay.Click += BtnChoiNgay_Click;

            btnCaiDat = TaoNutCoXua("⚙️ Cài Đặt", new Point(btnX, btnY + gap), BTN_BLUE);
            btnCaiDat.Click += BtnCaiDat_Click;

            btnLuatChoi = TaoNutCoXua("📜 Luật Chơi", new Point(btnX, btnY + gap * 2), BTN_ORANGE);
            btnLuatChoi.Click += BtnLuatChoi_Click;

            btnThoat = TaoNutCoXua("❌ Thoát", new Point(btnX, btnY + gap * 3), BTN_RED);
            btnThoat.Click += (s, e) => Application.Exit();

            // Nút Tiếp Tục Chơi (ẩn mặc định, chỉ hiện khi có game đang tạm dừng)
            btnTiepTuc = TaoNutCoXua("▶ Tiếp Tục Chơi", new Point(btnX, btnY + gap * 4), BTN_GREEN);
            btnTiepTuc.Click += BtnTiepTuc_Click;
            btnTiepTuc.Visible = false;

            //Panel Cài Đặt
            pnlCaiDat = new ClassicPanel
            {
                Location = new Point(45, 450),
                Size = new Size(430, 240),
                Visible = false
            };

            // Nội dung Panel Cài Đặt
            Label lblCaiDatTitle = new Label
            {
                Text = "⚙  CÀI ĐẶT",
                Font = new Font("Georgia", 10, FontStyle.Bold),
                ForeColor = WOOD_DARK,
                AutoSize = true,
                Location = new Point(145, 15),
                BackColor = Color.Transparent
            };

            Label lblDoKho = TaoLabel("Độ Khó (AI):", new Point(30, 60));
            cbxDoKho = TaoComboBox(new Point(185, 57));
            cbxDoKho.Items.AddRange(new string[] { "De", "Binh thuong", "Ke huy diet" });
            cbxDoKho.SelectedIndex = 1;

            Label lblKichThuoc = TaoLabel("Kích Thước Bàn:", new Point(30, 105));
            cbxKichThuoc = TaoComboBox(new Point(185, 102));
            cbxKichThuoc.Items.AddRange(new string[] { "7 x 7", "10 x 10" });
            cbxKichThuoc.SelectedIndex = 0;

            Label lblAmLuong = TaoLabel("Âm Lượng:", new Point(30, 155));
            trbAmLuong = new TrackBar
            {
                Location = new Point(180, 150),
                Size = new Size(190, 45),
                Minimum = 0,
                Maximum = 100,
                Value = 50,
                TickFrequency = 10,
                BackColor = Color.FromArgb(245, 235, 215)
            };

            lblAmLuongValue = new Label
            {
                Text = "50%",
                Font = new Font("Georgia", 10, FontStyle.Bold),
                ForeColor = WOOD_DARK,
                Location = new Point(375, 157),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            pnlCaiDat.Controls.AddRange(new Control[] {
                lblCaiDatTitle, lblDoKho, cbxDoKho, lblKichThuoc, cbxKichThuoc,
                lblAmLuong, trbAmLuong, lblAmLuongValue
            });

            // Thêm tất cả vào Form
            this.Controls.AddRange(new Control[] {
                lblTieuDe, lblPhuDe,
                btnChoiNgay, btnCaiDat, btnLuatChoi, btnThoat,
                btnTiepTuc, pnlCaiDat
            });
        }

        // ==================== FACTORY METHODS ====================

        private ClassicButton TaoNutCoXua(string ten, Point viTri, Color mauNen)
        {
            return new ClassicButton
            {
                Text = ten,
                Location = viTri,
                Size = new Size(240, 50),
                BaseColor = mauNen,
                AccentColor = Color.FromArgb(
                    Math.Min(255, mauNen.R + 30),
                    Math.Min(255, mauNen.G + 30),
                    Math.Min(255, mauNen.B + 30))
            };
        }

        private Label TaoLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Georgia", 10, FontStyle.Bold),
                ForeColor = INK_DARK,
                AutoSize = true,
                Location = location,
                BackColor = Color.Transparent
            };
        }

        private ComboBox TaoComboBox(Point location)
        {
            var cbx = new ComboBox
            {
                Location = location,
                Size = new Size(200, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Georgia", 10),
                BackColor = CREAM,
                ForeColor = INK_DARK
            };
            return cbx;
        }

        // ==================== SỰ KIỆN ====================

        private void BtnCaiDat_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            pnlCaiDat.Visible = !pnlCaiDat.Visible;
            btnCaiDat.Text = pnlCaiDat.Visible ? "⚙️ ĐÓNG CÀI ĐẶT" : "⚙️  CÀI ĐẶT";
        }

        private void BtnLuatChoi_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            string luat = "LUẬT CHƠI CỜ CARO:\n\n" +
                          "1. Hai bên thay phiên nhau đánh dấu X và O lên bàn cờ.\n" +
                          "2. Bên nào tạo được một đường thẳng, ngang, hoặc chéo gồm 5 ô liên tiếp sẽ chiến thắng.\n" +
                          "3. Bạn chỉ có 12 giây để suy nghĩ cho mỗi nước đi. Nếu hết giờ, bạn sẽ bị xử thua!\n\n" +
                          "Chúc bạn may mắn!";
            MessageBox.Show(luat, "Luật Chơi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnChoiNgay_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            MoGameMoi();
        }

        // Tạo và mở một ván game mới với các thông số hiện tại
        private void MoGameMoi()
        {
            // Đóng game cũ nếu có
            if (_currentGame != null && !_currentGame.IsDisposed)
            {
                _currentGame.FormClosed -= CurrentGame_FormClosed;
                _currentGame.Close();
                _currentGame = null;
            }

            // Lấy thông số từ UI
            int kichThuoc = 7;
            if (cbxKichThuoc.SelectedIndex == 1) kichThuoc = 10;
            int doKho = cbxDoKho.SelectedIndex + 1;
            int amLuong = trbAmLuong.Value;

            // Tạo game mới
            _currentGame = new Form1(kichThuoc, kichThuoc, doKho, amLuong);

            // Khi người chơi bấm "Quay lại Menu" trong game
            _currentGame.OnMenuRequested += () =>
            {
                CapNhatNutMenu();
                this.Show();
            };

            // Khi game bị đóng hoàn toàn (nút X)
            _currentGame.FormClosed += CurrentGame_FormClosed;

            this.Hide();
            _currentGame.Show();
        }

        private void CurrentGame_FormClosed(object sender, FormClosedEventArgs e)
        {
            _currentGame = null;
            CapNhatNutMenu();
            this.Show();
        }

        // --- NúT TIẾP TỤC CHƠI ---
        private void BtnTiepTuc_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            if (_currentGame != null && !_currentGame.IsDisposed)
            {
                this.Hide();
                _currentGame.Show();
                _currentGame.TiepTuc();
            }
        }

        // Cập nhật hiển thị nút Tiếp tục / Chơi mới (chỉ hiện khi có game đang tạm dừng)
        private void CapNhatNutMenu()
        {
            bool hasGame = _currentGame != null && !_currentGame.IsDisposed;
            btnTiepTuc.Visible = hasGame;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _sparkleTimer?.Stop();
            _sparkleTimer?.Dispose();
            base.OnFormClosed(e);
        }
    }
}