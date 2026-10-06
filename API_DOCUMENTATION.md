# TÀI LIỆU API - SPORT NEXUS BACKEND

Tài liệu này mô tả danh sách các API chính và luồng hoạt động (Workflow) của hệ thống Sport Nexus.

---
-acc: admin1 mk: admin1/acc: User1 mk: user1/acc: CourtOwner1 mk: courtowner1.
## 1. Authentication (Xác thực & Phân quyền)
- `POST /api/auth/login`: Đăng nhập hệ thống, trả về JWT Token.
- `POST /api/auth/register`: Đăng ký tài khoản (Mặc định Role: User).
- `POST /api/auth/register-admin`: Đăng ký tài khoản Admin (Dùng để test).

## 2. Quản lý Cơ sở vật chất (Facility & Court) - *Dành cho Court Owner*
- **FacilityController**:
  - `GET /api/v1/facilities`: Lấy danh sách khu sân (có lọc theo tên, địa chỉ).
  - `POST /api/v1/facilities`: Đăng ký khu sân mới.
  - `PUT /api/v1/facilities/{id}`: Cập nhật thông tin khu sân.
- **CourtController**:
  - `POST /api/v1/courts`: Thêm một sân lẻ (Court) vào khu sân (Facility).
- **MasterDataController**:
  - Lấy danh sách tiện ích (Amenities), Môn thể thao (Sport Categories).

## 3. Đặt sân & Thanh toán (Booking Flow)
- **CourtSlotController**:
  - `GET /api/v1/courtslots`: Lấy danh sách các khung giờ trống của một sân trong ngày.
  - `POST /api/v1/courtslots/generate`: (Chủ sân) Tự động sinh ra các khung giờ trống dựa vào giờ mở/đóng cửa.
- **BookingController**:
  - `POST /api/v1/bookings`: Tạo đơn đặt sân. 
  - `POST /api/v1/bookings/{id}/cancel`: Hủy đặt sân.
- **PaymentController**:
  - `POST /api/v1/payments/webhook`: Nhận IPN (Webhook) từ cổng thanh toán (VNPay/Momo) để cập nhật trạng thái đơn hàng.

**🔄 Luồng hoạt động (Booking & Split Payment Workflow):**
1. Host (Người đặt) tìm sân trống qua `CourtSlotController`.
2. Host tiến hành Booking, hệ thống giữ chỗ (Pending).
3. Host thanh toán 100% tiền cọc (Chuyển khoản / Cổng thanh toán).
4. Hệ thống nhận Webhook, đổi trạng thái Booking thành `Confirmed`.
5. Bạn bè của Host sẽ dùng Ví nội bộ (Internal Wallet) để trả lại tiền chia sẻ (Split Payment) trực tiếp cho Host.

---

## 4. Ghép kèo & Tìm đối (Social & LFG)
- **SocialController**:
  - `GET /api/v1/social/posts`: Lấy bảng tin cộng đồng (Feed).
  - `POST /api/v1/social/posts`: Đăng bài tìm đối / giao lưu.
  - `POST /api/v1/social/lfg`: Đăng thẻ LFG (Looking For Group) có thu phí.

**🔄 Luồng hoạt động LFG (Ký quỹ bằng Ví):**
1. Chủ LFG tạo kèo (Cần 3 slot, mỗi slot 50k).
2. Người chơi claim (nhận) slot -> Hệ thống trừ 50k từ Ví của họ và đưa vào `FrozenBalance` (Tiền đóng băng).
3. Nếu kèo đủ người -> Tiền đóng băng chuyển sang Ví của Chủ LFG. Nếu kèo thất bại / bị hủy -> Tiền đóng băng tự động hoàn trả về Ví người chơi.

---

## 5. Tổ chức Giải đấu (Tournament)
- **TournamentController**:
  - `POST /api/v1/tournaments`: Tạo giải đấu mới.
  - `POST /api/v1/tournaments/{id}/join`: Đăng ký tham gia giải đấu.
  - `POST /api/v1/tournaments/{id}/generate-bracket`: Random chia bảng/nhánh đấu.

**🔄 Luồng hoạt động Bracket (Nhánh đấu Binary Tree):**
1. Admin / Chủ giải đấu ấn chốt danh sách.
2. Hệ thống đếm số người/đội tham gia, sinh ra cây nhánh đấu (Bracket Matches).
3. Index 1 là Chung kết, Index 2-3 là Bán kết. Người chiến thắng ở vòng ngoài sẽ tiến vào vòng trong (công thức `CurrentIndex / 2`).

---

## 6. Check-In bằng QR Code & Fair Play
- **CheckInController**:
  - `POST /api/v1/checkin/scan`: Quét mã QR tại sân.
- **Fair Play (Nằm chung CheckIn / Social)**: Đánh giá thái độ người chơi.

**🔄 Luồng hoạt động Check-In:**
1. Khi đơn đặt sân thành công, hệ thống sinh ra một QR Code (Polymorphic QR).
2. Tới sân, Chủ sân dùng app quét QR.
3. Hệ thống ghi nhận `IsCheckedIn = true`. 
4. **Lưu ý:** User chỉ được phép đánh giá (Review/Fair Play) những người có cùng trạng thái `IsCheckedIn = true` trong trận đó để chống spam.

---

## 7. Trợ lý Ảo (AI Assistant)
- **AiAssistantController**:
  - `POST /api/v1/ai/chat`: Chat với trợ lý ảo (Hỏi luật chơi, tư vấn sân bãi). Lịch sử chat được lưu trong `AiChatSessions` và `AiPromptLogs`.

---

## 8. Development Tools (Môi trường Dev)
- **SeederController**:
  - `POST /api/v1/seeder/run`: Tạo dữ liệu ảo 29 bảng.
  - Mã bảo mật: Cần truyền `?secret=SEP490_ADMIN_SECRET_KEY`

---
*Tài liệu này được tạo tự động để hỗ trợ team Frontend và Giám khảo hiểu luồng đi của dữ liệu trong đồ án SEP490.*

---

## 9. DANH SÁCH TOÀN BỘ API CHI TIẾT (FULL ENDPOINTS)

Dưới đây là danh sách toàn bộ các API đã được code trong Backend:

### 👤 AccountController
- **GET** `/api/accounts`
- **GET** `/api/accounts/{id:guid}`
- **POST** `/api/accounts`
- **PUT** `/api/accounts/{id:guid}`
- **DELETE** `/api/accounts/{id:guid}`

### 🤖 AiAssistantController
- **POST** `/api/v1/ai/sessions` : Tạo phiên chat mới
- **GET** `/api/v1/ai/sessions` : Lấy lịch sử chat
- **GET** `/api/v1/ai/sessions/{id}/messages` : Lấy tin nhắn trong 1 phiên
- **POST** `/api/v1/ai/sessions/{id}/chat` : Chat với AI

### 🔐 AuthController
- **POST** `/api/auth/register` : Đăng ký User
- **POST** `/api/auth/register-admin` : Đăng ký Admin
- **POST** `/api/auth/login` : Đăng nhập (Trả về JWT)

### 💳 PaymentController & NotificationController (Xử lý giao dịch & Thông báo)
- **POST** `/api/v1/payments/webhook` : Nhận Webhook từ cổng thanh toán
- **GET** `/api/v1/notifications` : Lấy danh sách thông báo
- **PUT** `/api/v1/notifications/{id}/read` : Đánh dấu đã đọc

### 📅 BookingController
- **POST** `/api/v1/bookings` : Tạo đơn đặt sân
- **POST** `/api/v1/bookings/{bookingId}/join` : Tham gia vào đơn đặt sân (Split Payment)
- **GET** `/api/v1/bookings/{bookingId}/qr-ticket` : Lấy vé QR Code

### 📸 CheckInController
- **POST** `/api/v1/checkin/scan` : Quét mã QR tại sân
- **POST** `/api/v1/checkin/fair-play` : Đánh giá người chơi sau trận
- **POST** `/api/v1/checkin/facility` : Đánh giá chất lượng sân

### 🏸 CourtController
- **GET** `/api/v1/facilities/{facilityId}/courts` : Lấy danh sách sân
- **POST** `/api/v1/facilities/{facilityId}/courts` : Thêm sân mới
- **PUT** `/api/v1/facilities/{facilityId}/courts/{courtId}` : Sửa sân
- **DELETE** `/api/v1/facilities/{facilityId}/courts/{courtId}` : Xóa sân

### 🕒 CourtSlotController
- **GET** `/api/v1/courts/{courtId}/slots` : Xem giờ trống
- **POST** `/api/v1/courts/{courtId}/slots/generate` : (Owner) Tự động sinh giờ trống
- **PUT** `/api/v1/courts/api/v1/slots/{slotId}/status` : Đổi trạng thái giờ (Bảo trì/Khóa)

### 🏢 FacilityController
- **GET** `/api/v1/facilities` : Tìm kiếm khu thể thao
- **GET** `/api/v1/facilities/{id}` : Xem chi tiết khu thể thao
- **PUT** `/api/v1/facilities/{id}` : Cập nhật thông tin

### 📚 MasterDataController
- **GET** `/api/v1/master-data/sports` : Danh sách môn thể thao
- **GET** `/api/v1/master-data/amenities` : Danh sách tiện ích (Wifi, Trà đá...)

### 🛠️ SeederController
- **POST** `/api/v1/seeder/run` : Sinh dữ liệu ảo (Yêu cầu Secret Key)

### 🤝 SocialController (Mạng xã hội & LFG)
- **POST** `/api/v1/lfgs` : Tạo thẻ Tìm nhóm (LFG)
- **POST** `/api/v1/lfgs/{cardId}/claim` : Nhận slot vào nhóm
- **POST** `/api/v1/lfgs/posts` : Đăng bài feed
- **POST** `/api/v1/lfgs/posts/{postId}/comments` : Bình luận

### 🏆 TournamentController
- **POST** `/api/v1/tournaments` : Tạo giải đấu
- **GET** `/api/v1/tournaments` : Xem danh sách giải
- **GET** `/api/v1/tournaments/{id}/bracket` : Xem sơ đồ nhánh đấu (Tree)
- **POST** `/api/v1/tournaments/{id}/join` : Đăng ký thi đấu
- **POST** `/api/v1/tournaments/{id}/execute-random-pairing` : Random ghép đội (Đánh đôi)
- **POST** `/api/v1/tournaments/{id}/generate-bracket` : Chốt sổ & Sinh nhánh đấu
- **PUT** `/api/v1/tournaments/{matchId}/score` : Cập nhật tỉ số trận đấu

