using System;
using System.IO;
using System.Windows.Media;

namespace GAMECARO7X7
{
    public static class C_AmThanh
    {
        // Tạo các luồng phát độc lập để âm thanh đè lên nhau không bị ngắt
        private static MediaPlayer playerNhacNen = new MediaPlayer();
        private static MediaPlayer playerHieuUng = new MediaPlayer();
        private static MediaPlayer playerDongHo = new MediaPlayer();
        private static MediaPlayer playerKetThuc = new MediaPlayer();

        // Lấy đường dẫn tới thư mục Sounds trong bin\Debug
        private static string thuMucSounds = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds");

        private static float _amLuong = 0.5f; // Mặc định 50%

        // Hàm cập nhật âm lượng từ thanh TrackBar của Menu
        public static void SetAmLuong(int phanTram)
        {
            _amLuong = phanTram / 100f;
            playerNhacNen.Volume = _amLuong * 0.4f; // Nhạc nền cho nhỏ hơn hiệu ứng một chút
            playerHieuUng.Volume = _amLuong;
            playerDongHo.Volume = _amLuong;
            playerKetThuc.Volume = _amLuong;
        }

        // --- CÁC HÀM PHÁT NHẠC ---

        public static void PhatNhacNen()
        {
            string path = Path.Combine(thuMucSounds, "nhac_nen.mp3");
            if (File.Exists(path))
            {
                playerNhacNen.Open(new Uri(path));
                // Lặp lại nhạc nền khi hết
                playerNhacNen.MediaEnded += (sender, e) => { playerNhacNen.Position = TimeSpan.Zero; playerNhacNen.Play(); };
                playerNhacNen.Play();
            }
        }

        public static void PhatClickButton() => PhatHieuUng("click_btn.mp3");

        public static void PhatDanhCo() => PhatHieuUng("danh_co.mp3");

        public static void PhatTick()
        {
            string path = Path.Combine(thuMucSounds, "tick.mp3");
            if (File.Exists(path))
            {
                playerDongHo.Open(new Uri(path));
                playerDongHo.Play();
            }
        }

        public static void PhatThang()
        {
            string path = Path.Combine(thuMucSounds, "thang.mp3");
            if (File.Exists(path)) { playerKetThuc.Open(new Uri(path)); playerKetThuc.Play(); }
        }

        public static void PhatThua()
        {
            string path = Path.Combine(thuMucSounds, "thua.mp3");
            if (File.Exists(path)) { playerKetThuc.Open(new Uri(path)); playerKetThuc.Play(); }
        }

        // Hàm để phát hiệu ứng chung (click, đánh cờ)
        private static void PhatHieuUng(string tenFile)
        {
            string path = Path.Combine(thuMucSounds, tenFile);
            if (File.Exists(path))
            {
                playerHieuUng.Open(new Uri(path));
                playerHieuUng.Play();
            }
        }
    }
}