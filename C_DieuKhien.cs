using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GAMECARO7X7
{
    public class C_DieuKhien
    {
        public static Pen pen;

        private Random rd = new Random();
        private C_BanCo BanCo;
        private C_OCo[,] MangOCo;

        public int CheDoChoi { get; set; }
        public int DoKho { get; set; }
        private int _luotDi;
        private int _sanSang;
        public bool SanSang
        {
            get { return _sanSang == 1; }
            set { _sanSang = value ? 1 : 0; }
        }

        private Stack<C_OCo> _stackCacNuocDaDi;
        private Stack<C_OCo> _stackRedo = new Stack<C_OCo>(); // Stack cho chế độ xem lại
        public bool DaKetThuc { get; set; } = false; // Game đã kết thúc (thắng/thua/hòa)

        // Các hướng duyệt: Ngang, Dọc, Chéo xuôi, Chéo ngược
        private readonly int[] dx = { 0, 1, 1, 1 };
        private readonly int[] dy = { 1, 0, 1, -1 };

        public C_DieuKhien(int soDong, int soCot, int doKho)
        {
            pen = new Pen(Color.Black);
            BanCo = new C_BanCo(soDong, soCot);
            MangOCo = new C_OCo[soDong, soCot];
            _stackCacNuocDaDi = new Stack<C_OCo>();
            DoKho = doKho; // Lưu lại độ khó
        }

        public void VeBanCo(Graphics g) => BanCo.VeBanCo(g);

        public void KhoiTaoMangOCo()
        {
            for (int i = 0; i < BanCo.SoDong; i++)
                for (int j = 0; j < BanCo.SoCot; j++)
                    MangOCo[i, j] = new C_OCo(i, j, 0);
        }

        public void danhCo(Graphics g, int mouseX, int mouseY)
        {
            int dong = mouseY / C_OCo.CHIEU_CAO;
            int cot = mouseX / C_OCo.CHIEU_RONG;

            if (dong < 0 || dong >= BanCo.SoDong || cot < 0 || cot >= BanCo.SoCot) return;

            if (MangOCo[dong, cot].SoHuu == 0)
            {
                BanCo.VeQuanCo(g, cot * C_OCo.CHIEU_RONG, dong * C_OCo.CHIEU_CAO, _luotDi);
                MangOCo[dong, cot].SoHuu = _luotDi;
                _stackCacNuocDaDi.Push(new C_OCo(dong, cot, _luotDi));
                _luotDi = _luotDi == 1 ? 2 : 1;
            }
        }

        public void VeLaiQuanCo(Graphics g)
        {
            foreach (C_OCo oco in _stackCacNuocDaDi)
                BanCo.VeQuanCo(g, oco.Cot * C_OCo.CHIEU_RONG, oco.Dong * C_OCo.CHIEU_CAO, oco.SoHuu);

            // Đánh dấu nước đi cuối cùng bằng viền màu nổi bật
            if (_stackCacNuocDaDi.Count > 0)
            {
                C_OCo lastMove = _stackCacNuocDaDi.Peek();
                int x = lastMove.Cot * C_OCo.CHIEU_RONG;
                int y = lastMove.Dong * C_OCo.CHIEU_CAO;

                // Nền nhấn nhẹ
                using (SolidBrush hlBrush = new SolidBrush(Color.FromArgb(50, 255, 215, 0))) // Vàng mờ
                    g.FillRectangle(hlBrush, x + 1, y + 1, C_OCo.CHIEU_RONG - 2, C_OCo.CHIEU_CAO - 2);

                // Viền đỏ nổi bật
                using (Pen hlPen = new Pen(Color.Red, 3f))
                    g.DrawRectangle(hlPen, x + 1, y + 1, C_OCo.CHIEU_RONG - 2, C_OCo.CHIEU_CAO - 2);
            }
        }

        public void ChoiVoiNguoi(Graphics g)
        {
            CheDoChoi = 1;
            _luotDi = rd.Next(1, 3);
            MessageBox.Show(_luotDi == 1 ? "Lượt đi đầu tiên là O" : "Lượt đi đầu tiên là X");
            _sanSang = 1;
            DaKetThuc = false;
            KhoiTaoMangOCo();
            _stackCacNuocDaDi.Clear();
            _stackRedo.Clear();
            VeBanCo(g);
        }

        public void choiVoiMay(Graphics g)
        {
            CheDoChoi = 2;
            _luotDi = rd.Next(1, 3);
            MessageBox.Show(_luotDi == 1 ? "Máy đi trước (quân O)" : "Người chơi đi trước (quân X)");
            _sanSang = 1;
            DaKetThuc = false;
            KhoiTaoMangOCo();
            _stackCacNuocDaDi.Clear();
            _stackRedo.Clear();
            VeBanCo(g);

            if (_luotDi == 1) mayDanh(g);
        }

        // ========================== THUẬT TOÁN AI ==========================

        // Bảng điểm theo số quân trong cửa sổ 5 ô (tăng theo cấp số nhân)
        private static readonly int[] DIEM_CUA_SO = { 0, 1, 15, 200, 5000, 100000 };


        // Đếm số quân liên tiếp của player từ (r,c) theo hướng (stepR, stepC)
        private int DemQuan(int r, int c, int stepR, int stepC, int player)
        {
            int count = 0;
            int i = r + stepR;
            int j = c + stepC;

            while (i >= 0 && i < BanCo.SoDong && j >= 0 && j < BanCo.SoCot && MangOCo[i, j].SoHuu == player)
            {
                count++;
                i += stepR;
                j += stepC;
            }
            return count;
        }


        // Kiểm tra ô (r,c) có nằm trong phạm vi radius ô chứa quân cờ không
        private bool IsNear(int r, int c, int radius)
        {
            int rMin = Math.Max(0, r - radius);
            int rMax = Math.Min(BanCo.SoDong - 1, r + radius);
            int cMin = Math.Max(0, c - radius);
            int cMax = Math.Min(BanCo.SoCot - 1, c + radius);

            for (int i = rMin; i <= rMax; i++)
                for (int j = cMin; j <= cMax; j++)
                    if (MangOCo[i, j].SoHuu != 0) return true;
            return false;
        }


        // Kiểm tra ô (r,c) có hợp lệ và trống không
        private bool OTrong(int r, int c)
        {
            return r >= 0 && r < BanCo.SoDong && c >= 0 && c < BanCo.SoCot && MangOCo[r, c].SoHuu == 0;
        }


        // Trả điểm cho mẫu (pattern) dựa trên số quân liên tiếp và số đầu mở.
        // Đầu mở = đầu liền kề là ô trống (có thể mở rộng thêm).
        private int DiemMau(int soQuan, int dauMo)
        {
            if (soQuan >= 5) return 100000;   // Thắng
            if (dauMo == 0) return 0;          // Bị chặn 2 đầu -> vô giá trị

            switch (soQuan)
            {
                case 4: return dauMo == 2 ? 50000 : 5000;   // Tứ mở vs Tứ nửa mở
                case 3: return dauMo == 2 ? 4000 : 400;     // Tam mở vs Tam nửa mở
                case 2: return dauMo == 2 ? 150 : 30;       // Nhị mở vs Nhị nửa mở
                case 1: return dauMo == 2 ? 10 : 2;         // Đơn mở vs Đơn nửa mở
                default: return 0;
            }
        }

        // -------------------- ĐÁNH GIÁ BÀN CờX (Window-based) --------------------
        // Đánh giá toàn bộ bàn cờ bằng phương pháp quét cửa sổ 5 ô.
        // Duyệt tất cả cửa sổ 5 ô liên tiếp theo 4 hướng.
        // Mỗi cửa sổ chỉ được tính 1 lần -> không bị đếm trùng.
        // Score dương = máy (player 1) có lợi, Score âm = người (player 2) có lợi.
        private int DanhGiaBanCo()
        {
            int score = 0;
            int rows = BanCo.SoDong, cols = BanCo.SoCot;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Duyệt 4 hướng: Ngang(0,1), Dọc(1,0), Chéo xuôi(1,1), Chéo ngược(1,-1)
                    for (int d = 0; d < 4; d++)
                    {
                        // Kiểm tra cửa sổ 5 ô có nằm trong bàn cờ không
                        int endR = i + 4 * dx[d];
                        int endC = j + 4 * dy[d];
                        if (endR < 0 || endR >= rows || endC < 0 || endC >= cols) continue;

                        int p1 = 0, p2 = 0;
                        for (int k = 0; k < 5; k++)
                        {
                            int owner = MangOCo[i + k * dx[d], j + k * dy[d]].SoHuu;
                            if (owner == 1) p1++;
                            else if (owner == 2) p2++;
                        }

                        // Cửa sổ chỉ có giá trị khi chứa quân của 1 bên duy nhất
                        if (p1 > 0 && p2 == 0)
                            score += DIEM_CUA_SO[p1];
                        else if (p2 > 0 && p1 == 0)
                            score -= DIEM_CUA_SO[p2];
                    }
                }
            }

            // Thưởng nhẹ cho vị trí trung tâm
            int centerR = rows / 2, centerC = cols / 2;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (MangOCo[i, j].SoHuu == 1)
                        score += (6 - Math.Abs(i - centerR) - Math.Abs(j - centerC));
                    else if (MangOCo[i, j].SoHuu == 2)
                        score -= (6 - Math.Abs(i - centerR) - Math.Abs(j - centerC));
                }
            }

            return score;
        }

        // -------------------- HEURISTIC NƯỚC ĐI (Move Ordering) --------------------

        // Tính điểm heuristic cho nước đi tại ô trống (r,c).
        // Xét cả tấn công (tạo mẫu cho máy) và phòng thủ (chặn mẫu đối thủ).
        // Dùng để sắp xếp nước đi -> cắt tỉa Alpha-Beta hiệu quả hơn.

        private int TinhDiemNuocDi(int r, int c)
        {
            int scoreAttack = 0;
            int scoreDefense = 0;

            for (int d = 0; d < 4; d++)
            {
                // === TẤN CÔNG: Mẫu tạo ra nếu máy (player 1) đặt tại (r,c) ===
                int aFwd = DemQuan(r, c, dx[d], dy[d], 1);
                int aBwd = DemQuan(r, c, -dx[d], -dy[d], 1);
                int aTotal = 1 + aFwd + aBwd; // Bao gồm quân vừa đặt

                int aOpen = 0;
                if (OTrong(r + (aFwd + 1) * dx[d], c + (aFwd + 1) * dy[d])) aOpen++;
                if (OTrong(r - (aBwd + 1) * dx[d], c - (aBwd + 1) * dy[d])) aOpen++;

                scoreAttack += DiemMau(aTotal, aOpen);

                // === PHÒNG THỦ: Mẫu đối thủ (player 2) bị chặn khi ta đặt tại (r,c) ===
                int dFwd = DemQuan(r, c, dx[d], dy[d], 2);
                int dBwd = DemQuan(r, c, -dx[d], -dy[d], 2);
                int dTotal = 1 + dFwd + dBwd; // Giả lập như đối thủ đặt ở đây

                int dOpen = 0;
                if (OTrong(r + (dFwd + 1) * dx[d], c + (dFwd + 1) * dy[d])) dOpen++;
                if (OTrong(r - (dBwd + 1) * dx[d], c - (dBwd + 1) * dy[d])) dOpen++;

                scoreDefense += DiemMau(dTotal, dOpen);
            }

            // Ưu tiên tấn công hơn phòng thủ một chút
            return scoreAttack * 2 + scoreDefense;
        }

        // -------------------- SINH NƯỚC ĐI TIỀM NĂNG --------------------


        // Lấy danh sách nước đi tiềm năng, sắp xếp theo heuristic giảm dần.
        // Bước 1: Lấy ô trống trong bán kính 1 quanh quân đã đặt.
        // Bước 2: Nếu quá ít, mở rộng bán kính lên 2.
        // Bước 3: Sắp xếp theo điểm heuristic, giới hạn số nước đi theo độ sâu.

        private List<C_OCo> LayDanhSachNuocDi(int maxMoves)
        {
            List<C_OCo> moves = new List<C_OCo>();

            // Bước 1: Bán kính 1
            for (int i = 0; i < BanCo.SoDong; i++)
                for (int j = 0; j < BanCo.SoCot; j++)
                    if (MangOCo[i, j].SoHuu == 0 && IsNear(i, j, 1))
                        moves.Add(new C_OCo(i, j, 0));

            // Bước 2: Mở rộng bán kính nếu quá ít ứng viên
            if (moves.Count < 5)
            {
                moves.Clear();
                for (int i = 0; i < BanCo.SoDong; i++)
                    for (int j = 0; j < BanCo.SoCot; j++)
                        if (MangOCo[i, j].SoHuu == 0 && IsNear(i, j, 2))
                            moves.Add(new C_OCo(i, j, 0));
            }

            // Bước 3: Sắp xếp theo heuristic (dùng mảng song song để tránh tính lại)
            int count = moves.Count;
            int[] scores = new int[count];
            for (int i = 0; i < count; i++)
                scores[i] = TinhDiemNuocDi(moves[i].Dong, moves[i].Cot);

            // Sắp xếp giảm dần bằng Selection Sort (hiệu quả cho N nhỏ)
            for (int i = 0; i < count - 1; i++)
            {
                int maxIdx = i;
                for (int j = i + 1; j < count; j++)
                    if (scores[j] > scores[maxIdx]) maxIdx = j;

                if (maxIdx != i)
                {
                    // Hoán đổi scores
                    int tmpScore = scores[i]; scores[i] = scores[maxIdx]; scores[maxIdx] = tmpScore;
                    // Hoán đổi moves
                    C_OCo tmpMove = moves[i]; moves[i] = moves[maxIdx]; moves[maxIdx] = tmpMove;
                }
            }

            // Giới hạn số nước đi ứng viên
            if (moves.Count > maxMoves)
                moves = moves.GetRange(0, maxMoves);

            return moves;
        }

        // -------------------- KIỂM TRA THẮNG NHANH --------------------

        // Kiểm tra player có thắng tại (r,c) không (đã đặt quân ở đó)
        private bool KiemTraThangMinimax(int r, int c, int player)
        {
            for (int i = 0; i < 4; i++)
                if (1 + DemQuan(r, c, dx[i], dy[i], player) + DemQuan(r, c, -dx[i], -dy[i], player) >= 5)
                    return true;
            return false;
        }

        // -------------------- MINIMAX + ALPHA-BETA --------------------

        // Thuật toán Minimax với cắt tỉa Alpha-Beta.
        // - isMax = true: lượt Máy (maximize), player 1
        // - isMax = false: lượt Người (minimize), player 2
        // - Kiểm tra thắng ngay khi đặt quân (terminal node)
        // - Giới hạn nước đi ứng viên theo độ sâu hiện tại
        // - Win score dùng depth bonus lớn (10000*depth) để ưu tiên thắng nhanh
        private int Minimax(int depth, bool isMax, int alpha, int beta)
        {
            // Đạt độ sâu tối đa -> đánh giá bàn cờ
            if (depth == 0) return DanhGiaBanCo();

            // Giới hạn nước đi theo độ sâu (ít hơn ở nhánh sâu để tăng tốc)
            int maxMoves = depth <= 2 ? 10 : 15;
            List<C_OCo> moves = LayDanhSachNuocDi(maxMoves);
            if (moves.Count == 0) return 0;

            if (isMax) // === LƯỢT MÁY (Player 1) - Maximize ===
            {
                int best = int.MinValue;
                foreach (C_OCo m in moves)
                {
                    MangOCo[m.Dong, m.Cot].SoHuu = 1;

                    // Kiểm tra thắng ngay -> trả về điểm cực lớn + bonus depth
                    if (KiemTraThangMinimax(m.Dong, m.Cot, 1))
                    {
                        MangOCo[m.Dong, m.Cot].SoHuu = 0;
                        return 1000000 + depth * 10000; // Thắng nhanh hơn = điểm cao hơn
                    }

                    int val = Minimax(depth - 1, false, alpha, beta);
                    MangOCo[m.Dong, m.Cot].SoHuu = 0;

                    if (val > best) best = val;
                    if (best > alpha) alpha = best;
                    if (beta <= alpha) break; // Cắt tỉa Beta
                }
                return best;
            }
            else // === LƯỢT NGƯỜI (Player 2) - Minimize ===
            {
                int best = int.MaxValue;
                foreach (C_OCo m in moves)
                {
                    MangOCo[m.Dong, m.Cot].SoHuu = 2;

                    // Kiểm tra thắng ngay cho người chơi
                    if (KiemTraThangMinimax(m.Dong, m.Cot, 2))
                    {
                        MangOCo[m.Dong, m.Cot].SoHuu = 0;
                        return -1000000 - depth * 10000; // Người thắng nhanh = điểm cực thấp
                    }

                    int val = Minimax(depth - 1, true, alpha, beta);
                    MangOCo[m.Dong, m.Cot].SoHuu = 0;

                    if (val < best) best = val;
                    if (best < beta) beta = best;
                    if (beta <= alpha) break; // Cắt tỉa Alpha
                }
                return best;
            }
        }

        // -------------------- MÁY ĐÁNH --------------------


        // Hàm chính để máy chọn nước đi.
        // Thứ tự ưu tiên:
        //   1) Nước đầu tiên -> đi gần trung tâm
        //   2) Thắng ngay -> đặt luôn
        //   3) Chặn đối thủ sắp thắng -> bắt buộc chặn
        //   4) Minimax + Alpha-Beta -> chọn nước tối ưu

        public void mayDanh(Graphics g)
        {
            if (_luotDi != 1) return;

            // --- Nước đầu tiên: đi gần trung tâm ---
            if (_stackCacNuocDaDi.Count == 0)
            {
                int centerR = BanCo.SoDong / 2;
                int centerC = BanCo.SoCot / 2;
                danhCo(g, centerC * C_OCo.CHIEU_RONG + 1, centerR * C_OCo.CHIEU_CAO + 1);
                return;
            }

            List<C_OCo> moves = LayDanhSachNuocDi(15);
            if (moves.Count == 0) return;

            // --- Ưu tiên 1: Kiểm tra nước thắng ngay cho máy ---
            foreach (C_OCo m in moves)
            {
                MangOCo[m.Dong, m.Cot].SoHuu = 1;
                bool win = KiemTraThangMinimax(m.Dong, m.Cot, 1);
                MangOCo[m.Dong, m.Cot].SoHuu = 0;
                if (win)
                {
                    danhCo(g, m.Cot * C_OCo.CHIEU_RONG + 1, m.Dong * C_OCo.CHIEU_CAO + 1);
                    return;
                }
            }

            // --- Ưu tiên 2: Chặn nước thắng ngay của đối thủ ---
            foreach (C_OCo m in moves)
            {
                MangOCo[m.Dong, m.Cot].SoHuu = 2;
                bool oppWin = KiemTraThangMinimax(m.Dong, m.Cot, 2);
                MangOCo[m.Dong, m.Cot].SoHuu = 0;
                if (oppWin)
                {
                    danhCo(g, m.Cot * C_OCo.CHIEU_RONG + 1, m.Dong * C_OCo.CHIEU_CAO + 1);
                    return;
                }
            }

            // --- Ưu tiên 3: Minimax + Alpha-Beta ---
            int depth = 3;
            if (DoKho == 1) depth = 2;      // Dễ: Nhìn trước 2 bước
            else if (DoKho == 2) depth = 4; // Trung bình: Nhìn trước 4 bước
            else if (DoKho == 3) depth = 6; // Khó: Nhìn trước 6 bước

            int bestScore = int.MinValue;
            C_OCo bestMove = moves[0]; // Mặc định lấy nước đi tốt nhất theo heuristic

            foreach (C_OCo m in moves)
            {
                MangOCo[m.Dong, m.Cot].SoHuu = 1;

                int score = Minimax(depth - 1, false, int.MinValue, int.MaxValue);
                MangOCo[m.Dong, m.Cot].SoHuu = 0;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = m;
                }
            }

            danhCo(g, bestMove.Cot * C_OCo.CHIEU_RONG + 1, bestMove.Dong * C_OCo.CHIEU_CAO + 1);
        }

        // ========================== KIỂM TRA THẮNG / THUA / VẼ ĐƯỜNG ==========================

        public bool KiemTraThang(Graphics g)
        {
            if (_stackCacNuocDaDi.Count == 0) return false;

            C_OCo lastMove = _stackCacNuocDaDi.Peek();
            int r = lastMove.Dong;
            int c = lastMove.Cot;
            int player = lastMove.SoHuu;

            for (int i = 0; i < 4; i++)
            {
                int front = DemQuan(r, c, dx[i], dy[i], player);
                int back = DemQuan(r, c, -dx[i], -dy[i], player);

                if (1 + front + back >= 5)
                {
                    // Tính tọa độ đầu và cuối để vẽ đường kẻ
                    int startR = r - back * dx[i];
                    int startC = c - back * dy[i];
                    int endR = r + front * dx[i];
                    int endC = c + front * dy[i];

                    int x1 = startC * C_OCo.CHIEU_RONG + C_OCo.CHIEU_RONG / 2;
                    int y1 = startR * C_OCo.CHIEU_CAO + C_OCo.CHIEU_CAO / 2;
                    int x2 = endC * C_OCo.CHIEU_RONG + C_OCo.CHIEU_RONG / 2;
                    int y2 = endR * C_OCo.CHIEU_CAO + C_OCo.CHIEU_CAO / 2;

                    g.DrawLine(new Pen(Color.Blue, 5f), x1, y1, x2, y2);

                    _sanSang = 2;
                    return true;
                }
            }
            return false;
        }

        // Kiểm tra hòa: tất cả ô đã được đánh mà không ai thắng

        public bool KiemTraHoa()
        {
            for (int i = 0; i < BanCo.SoDong; i++)
                for (int j = 0; j < BanCo.SoCot; j++)
                    if (MangOCo[i, j].SoHuu == 0)
                        return false; // Còn ô trống -> chưa hòa
            return true; // Tất cả ô đã đầy -> hòa
        }

        public void ThongBaoChienThang()
        {
            if (_stackCacNuocDaDi.Count == 0) return;
            int player = _stackCacNuocDaDi.Peek().SoHuu;
            if (CheDoChoi == 1)
                MessageBox.Show(player == 1 ? "Chúc mừng quân xanh chiến thắng!" : "Chúc mừng quân đỏ chiến thắng!");
            else
                MessageBox.Show(player == 1 ? "Máy đã chiến thắng!" : "Người chơi đã chiến thắng!");
        }

        public void Undo()
        {
            if (_stackCacNuocDaDi.Count == 0) return;
            C_OCo nuocVua = _stackCacNuocDaDi.Pop();
            MangOCo[nuocVua.Dong, nuocVua.Cot].SoHuu = 0;
            _luotDi = nuocVua.SoHuu;
        }

        // Undo chế độ xem lại: lùi nước đi và lưu vào stack redo
        public void UndoXemLai()
        {
            if (_stackCacNuocDaDi.Count == 0) return;
            C_OCo nuocVua = _stackCacNuocDaDi.Pop();
            MangOCo[nuocVua.Dong, nuocVua.Cot].SoHuu = 0;
        }
    }
}