# Game Caro 7x7 - Trí Tuệ Nhân Tạo (Minimax & Alpha-Beta Pruning)

**Đề tài 6 - Nhóm 3**  
Dự án môn học Trí tuệ nhân tạo (Artificial Intelligence), xây dựng trò chơi Caro kích thước bàn cờ 7x7, trong đó người chơi có thể thi đấu với máy (AI). Trí tuệ nhân tạo của máy được cài đặt dựa trên **Thuật toán Minimax** kết hợp với kỹ thuật **Cắt tỉa Alpha-Beta (Alpha-Beta Pruning)** để tối ưu hóa tốc độ tính toán và ra quyết định thông minh.

---

## 🎮 Tính năng chính

- **Chế độ chơi đa dạng**: Hỗ trợ người chơi đánh với máy (AI) hoặc chế độ hai người chơi.
- **AI thông minh**: Áp dụng thuật toán Minimax và tối ưu hóa bằng Alpha-Beta Pruning giúp máy tính có khả năng chặn các nước đi nguy hiểm và tìm đường chiến thắng.
- **Giao diện trực quan**: Được xây dựng bằng C# Windows Forms, thân thiện với người sử dụng.
- **Hệ thống âm thanh**: Tích hợp âm thanh khi đánh cờ hoặc khi thắng/thua để tăng trải nghiệm người chơi.

## 🛠 Công nghệ sử dụng

- **Ngôn ngữ lập trình**: C#
- **Nền tảng/Framework**: .NET Framework (Windows Forms Application)
- **Công cụ phát triển (IDE)**: Microsoft Visual Studio

## 📂 Cấu trúc dự án

- `GAMECARO7X7.sln`: File Solution chính của toàn bộ dự án.
- `GAMECARO7X7/`
  - `FormMenu.cs` & `Form1.cs`: Quản lý giao diện, menu chính và bàn cờ UI.
  - `C_DieuKhien.cs`: Lớp điều khiển chính, xử lý luồng trò chơi và tích hợp thuật toán AI (Minimax & Alpha-Beta).
  - `C_BanCo.cs`: Quản lý logic hiển thị và cập nhật trạng thái bàn cờ.
  - `C_OCo.cs`: Định nghĩa thuộc tính cho từng ô trên bàn cờ.
  - `C_AmThanh.cs`: Quản lý âm thanh và hiệu ứng trong game.
  - `HinhAnh/` & `Resources/`: Lưu trữ tài nguyên hình ảnh, biểu tượng.

## 🚀 Hướng dẫn cài đặt và chạy thử

1. **Yêu cầu hệ thống**: Cài đặt sẵn Microsoft Visual Studio (phiên bản 2017, 2019 hoặc 2022) có hỗ trợ khối lượng công việc (workload) **.NET desktop development**.
2. **Mở dự án**:
   - Clone kho lưu trữ này về máy hoặc giải nén nếu tải file `.zip`.
   - Mở thư mục dự án và nhấp đúp vào file `GAMECARO7X7.sln`.
3. **Chạy ứng dụng**:
   - Chờ Visual Studio tải xong các thành phần.
   - Bấm nút **Start** (hoặc phím `F5`) ở thanh công cụ phía trên để biên dịch và chạy game.
