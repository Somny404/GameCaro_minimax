using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GAMECARO7X7
{
    public partial class Form1 : Form
    {
        private int SO_DONG;
        private int SO_COT;
        private int doKhoAI; // Để lưu độ khó AI
        private PictureBox picMeme;

        private C_DieuKhien dieuKhien;
        private Panel panelBanCo;
        private Button btnChoiVoiNguoi, btnChoiVoiMay, btnUndo, btnMenu;
        private Label lblTrangThai, lblDemNguoc;
        private ProgressBar prbThoiGian;
        private Timer tmrDemNguoc;

        private int thoiGianConLai = 12; // 12 giây áp lực

        // Sự kiện báo cho Menu khi người chơi quay lại
        public event Action OnMenuRequested;

        //Màu giao dien
        private static readonly Color WOOD_DARK = Color.FromArgb(101, 67, 33);
        private static readonly Color WOOD_LIGHT = Color.FromArgb(181, 137, 80);
        private static readonly Color GOLD = Color.FromArgb(218, 165, 32);
        private static readonly Color INK_DARK = Color.FromArgb(45, 30, 15);

        public Form1(int soDong = 7, int soCot = 7, int doKho = 2, int amLuongHienTai = 50)
        {
            InitializeComponent();

            // Gán dữ liệu từ Menu chuyển sang
            SO_DONG = soDong;
            SO_COT = soCot;
            doKhoAI = doKho;

            KhoiTaoUI();
            dieuKhien = new C_DieuKhien(SO_DONG, SO_COT, doKhoAI);
        }

        // ==================== TRANG TRÍ GIAO DIỆN ====================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = ClientSize.Width;
            int h = ClientSize.Height;

            //Nền WhiteSmoke
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(248, 248, 248),
                Color.FromArgb(230, 230, 235),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            //trang trí cho giống giấy
            Random rng = new Random(42);
            using (Pen grainPen = new Pen(Color.FromArgb(10, 100, 100, 110), 0.5f))
            {
                for (int i = 0; i < 100; i++)
                {
                    int x1 = rng.Next(w);
                    int y1 = rng.Next(h);
                    g.DrawLine(grainPen, x1, y1, x1 + rng.Next(8, 30), y1 + rng.Next(-2, 3));
                }
            }

            //Khung viền gỗ bao quanh
            int border = 8;
            Rectangle frameOuter = new Rectangle(border, border, w - border * 2, h - border * 2);
            Rectangle frameInner = new Rectangle(border + 4, border + 4, w - border * 2 - 8, h - border * 2 - 8);

            using (Pen framePen = new Pen(WOOD_DARK, 3f))
                g.DrawRectangle(framePen, frameOuter);

            using (Pen innerPen = new Pen(Color.FromArgb(150, WOOD_LIGHT), 1.2f))
                g.DrawRectangle(innerPen, frameInner);

            //Trang trí 4 góc
            DrawOrnateCorner(g, border, border);
            DrawOrnateCorner(g, w - border - 22, border);
            DrawOrnateCorner(g, border, h - border - 22);
            DrawOrnateCorner(g, w - border - 22, h - border - 22);

            //Họa tiết quân cờ mờ ở góc
            using (Font chessFont = new Font("Georgia", 28, FontStyle.Bold))
            using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(15, INK_DARK)))
            {
                g.DrawString("✕", chessFont, fadeBrush, w - 55, h - 55);
                g.DrawString("○", chessFont, fadeBrush, 12, h - 55);
            }
        }

        private void DrawOrnateCorner(Graphics g, int x, int y)
        {
            using (Pen goldPen = new Pen(Color.FromArgb(180, GOLD), 1.8f))
            {
                g.DrawArc(goldPen, x, y, 18, 18, 180, 90);
                g.DrawLine(goldPen, x + 9, y, x + 20, y);
                g.DrawLine(goldPen, x, y + 9, x, y + 20);

                using (SolidBrush dotBrush = new SolidBrush(Color.FromArgb(140, GOLD)))
                {
                    g.FillEllipse(dotBrush, x + 7, y + 7, 4, 4);
                }
            }
        }

        private void KhoiTaoUI()
        {
            int boardW = SO_COT * C_OCo.CHIEU_RONG + 1;
            int boardH = SO_DONG * C_OCo.CHIEU_CAO + 1;

            // 1. Panel Bàn cờ
            panelBanCo = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(boardW, boardH),
                BackColor = Color.Wheat
            };

            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, panelBanCo, new object[] { true });

            panelBanCo.Paint += PanelBanCo_Paint;
            panelBanCo.MouseClick += PanelBanCo_MouseClick;

            // Tọa độ X cho cột bên phải
            int btnX = boardW + 20;

            // 2. Nhãn Trạng Thái
            lblTrangThai = new Label
            {
                Text = "TRẠNG THÁI:\nSẵn sàng...!",
                Location = new Point(btnX, 10),
                Size = new Size(200, 45),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // 3. Thanh Tiến Độ (ProgressBar)
            prbThoiGian = new ProgressBar
            {
                Location = new Point(btnX, 60),
                Size = new Size(200, 15),
                Maximum = 12,
                Value = 12,
                Style = ProgressBarStyle.Continuous
            };

            // 4. Số đếm ngược
            lblDemNguoc = new Label
            {
                Text = "0:12",
                Location = new Point(btnX, 80),
                Size = new Size(200, 40),
                Font = new Font("Consolas", 24, FontStyle.Bold),
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // 5. Cụm Nút Bấm
            btnChoiVoiNguoi = TaoBut("Người vs Người", new Point(btnX, 130));
            btnChoiVoiNguoi.Click += BtnChoiVoiNguoi_Click;

            btnChoiVoiMay = TaoBut("Người vs Máy", new Point(btnX, 175));
            btnChoiVoiMay.Click += BtnChoiVoiMay_Click;

            btnUndo = TaoBut("Undo", new Point(btnX, 220));
            btnUndo.BackColor = Color.LightCoral;
            btnUndo.Click += BtnUndo_Click;

            // NÚT MENU MỚI ĐƯỢC THÊM VÀO
            btnMenu = TaoBut("Quay lại Menu", new Point(btnX, 265));
            btnMenu.BackColor = Color.SeaGreen;
            btnMenu.Click += BtnMenu_Click;

            // 6. Ảnh meme áp lực
            picMeme = new PictureBox
            {
                Location = new Point(btnX + 25, 315),
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Properties.Resources.sick,
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(picMeme); // Thêm vào Form

            // 7. Timer logic
            tmrDemNguoc = new Timer { Interval = 1000 };
            tmrDemNguoc.Tick += TmrDemNguoc_Tick;

            // Thêm tất cả vào Form
            this.Controls.AddRange(new Control[] {
                panelBanCo, lblTrangThai, prbThoiGian, lblDemNguoc,
                btnChoiVoiNguoi, btnChoiVoiMay, btnUndo, btnMenu
            });

            this.Text = "Cờ Caro - Nhóm 3 - Chủ đề 6";
            this.ClientSize = new Size(boardW + 250, boardH + 20);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.DoubleBuffered = true;
            this.Icon = Properties.Resources.iconapp;
        }

        private Button TaoBut(string ten, Point viTri)
        {
            return new Button
            {
                Text = ten,
                Location = viTri,
                Size = new Size(200, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        //Logic đếm ngược thời gian
        private void TmrDemNguoc_Tick(object sender, EventArgs e)
        {
            thoiGianConLai--;

            if (thoiGianConLai >= 0)
            {
                lblDemNguoc.Text = $"0:{thoiGianConLai:D2}";
                prbThoiGian.Value = thoiGianConLai;
            }

            // Xử lý khi thời gian rơi vào 8 giây cuối cùng
            if (thoiGianConLai <= 8 && thoiGianConLai > 0)
            {
                C_AmThanh.PhatTick(); // Tiếng tíc tắc 8 giây cuối
                // ĐỔI ẢNH MEME SANG TRẠNG THÁI HOẢNG LOẠN
                picMeme.Image = Properties.Resources.omg;
            }
            // Xử lý khi hết giờ
            else if (thoiGianConLai == 0)
            {
                C_AmThanh.PhatThua();
                tmrDemNguoc.Stop();

                MessageBox.Show("Hết giờ! Bạn đã thua do quá chậm chạp", "Cảnh báo");
                dieuKhien.SanSang = false; // Khóa bàn cờ
                dieuKhien.DaKetThuc = true;
            }
        }

        private void DatLaiThoiGian()
        {
            thoiGianConLai = 12;
            lblDemNguoc.Text = "0:12";
            prbThoiGian.Value = 12;
            picMeme.Image = Properties.Resources.sick;
            tmrDemNguoc.Start();
        }

        //Các sự kiện tương tác
        private void PanelBanCo_Paint(object sender, PaintEventArgs e)
        {
            dieuKhien.VeBanCo(e.Graphics);
            dieuKhien.VeLaiQuanCo(e.Graphics);
        }

        private void PanelBanCo_MouseClick(object sender, MouseEventArgs e)
        {
            if (!dieuKhien.SanSang) return;

            using (Graphics g = panelBanCo.CreateGraphics())
            {
                dieuKhien.danhCo(g, e.X, e.Y);
                C_AmThanh.PhatDanhCo();
                DatLaiThoiGian(); // Reset thời gian sau khi đánh

                if (dieuKhien.KiemTraThang(g))
                {
                    tmrDemNguoc.Stop();
                    C_AmThanh.PhatThang();
                    dieuKhien.DaKetThuc = true;
                    dieuKhien.ThongBaoChienThang();
                    return;
                }

                // Kiểm tra hòa sau khi người đánh
                if (dieuKhien.KiemTraHoa())
                {
                    tmrDemNguoc.Stop();
                    dieuKhien.SanSang = false;
                    dieuKhien.DaKetThuc = true;
                    MessageBox.Show("Hòa! Bàn cờ đã đầy mà không ai thắng.", "Kết quả");
                    return;
                }

                if (dieuKhien.CheDoChoi == 2)
                {
                    dieuKhien.mayDanh(g);
                    DatLaiThoiGian(); // Reset thời gian cho người chơi sau khi máy đánh

                    if (dieuKhien.KiemTraThang(g))
                    {
                        tmrDemNguoc.Stop();
                        C_AmThanh.PhatThang();
                        dieuKhien.DaKetThuc = true;
                        dieuKhien.ThongBaoChienThang();
                    }
                    // Kiểm tra hòa sau khi máy đánh
                    else if (dieuKhien.KiemTraHoa())
                    {
                        tmrDemNguoc.Stop();
                        dieuKhien.SanSang = false;
                        dieuKhien.DaKetThuc = true;
                        MessageBox.Show("Hòa! Bàn cờ đã đầy mà không ai thắng.", "Kết quả");
                    }
                }
            }
            panelBanCo.Invalidate();
        }

        private void BtnChoiVoiNguoi_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            using (Graphics g = panelBanCo.CreateGraphics()) dieuKhien.ChoiVoiNguoi(g);
            panelBanCo.Refresh();
            lblTrangThai.Text = "TRẠNG THÁI:\nNgười vs Người";
            DatLaiThoiGian();
        }

        private void BtnChoiVoiMay_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            using (Graphics g = panelBanCo.CreateGraphics()) dieuKhien.choiVoiMay(g);
            panelBanCo.Refresh();
            lblTrangThai.Text = "TRẠNG THÁI:\nNgười vs Máy";
            DatLaiThoiGian();
        }

        private void BtnUndo_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();

            if (dieuKhien.DaKetThuc)
            {
                // Chế độ xem lại: chỉ lùi nước đi, KHÔNG cho chơi tiếp
                dieuKhien.UndoXemLai();
            }
            else
            {
                // Chế độ đang chơi: undo bình thường, cho chơi tiếp
                dieuKhien.Undo();
                if (dieuKhien.CheDoChoi == 2) dieuKhien.Undo();
                dieuKhien.SanSang = true;
                DatLaiThoiGian();
            }
            panelBanCo.Invalidate();
        }

        // --- SỰ KIỆN NÚT MENU ---
        private void BtnMenu_Click(object sender, EventArgs e)
        {
            C_AmThanh.PhatClickButton();
            tmrDemNguoc.Stop(); // Tạm dừng đồng hồ
            this.Hide(); // Ẩn form thay vì đóng để giữ trạng thái
            OnMenuRequested?.Invoke();
        }
 
        // Tiếp tục game sau khi quay lại từ menu
        public void TiepTuc()
        {
            if (dieuKhien.SanSang)
                tmrDemNguoc.Start(); // Tiếp tục đếm ngược
        }
    }
}