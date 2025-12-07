# ĐẶC TẢ DỰ ÁN - HỆ THỐNG QUẢN LÝ KHÓA HỌC TRỰC TUYẾN

## 📋 MỤC LỤC

1. [Tổng quan Dự án](#1-tổng-quan-dự-án)
2. [Actors và Quyền hạn](#2-actors-và-quyền-hạn)
3. [Use Cases Chi tiết](#3-use-cases-chi-tiết)
4. [Quy trình Nghiệp vụ](#4-quy-trình-nghiệp-vụ)
5. [Cấu trúc Database](#5-cấu-trúc-database)
6. [API Specifications](#6-api-specifications)
7. [Business Rules](#7-business-rules)
8. [Validation Rules](#8-validation-rules)
9. [Error Handling](#9-error-handling)
10. [UI/UX Requirements](#10-uiux-requirements)

---

## 1. TỔNG QUAN DỰ ÁN

### 1.1. Mô tả Dự án

Hệ thống quản lý khóa học trực tuyến cho phép:
- Học viên đăng ký và học các khóa học trực tuyến
- Giảng viên tạo và quản lý khóa học của mình
- Kiểm duyệt viên duyệt yêu cầu đăng ký làm giảng viên
- Admin quản lý toàn bộ hệ thống

### 1.2. Mô hình Kinh doanh

- Học viên thanh toán một lần cho mỗi khóa học
- Doanh thu được chia: **70% cho Giảng viên, 30% cho Admin**
- Học viên có thời hạn truy cập khóa học sau khi thanh toán (ví dụ: 6 tháng, 12 tháng)
- Sau khi hết thời hạn, học viên không thể truy cập khóa học và phải gia hạn bằng cách thanh toán lại
- Khóa học không có thời hạn tự động hết hạn, tồn tại vĩnh viễn trong hệ thống

### 1.3. Công nghệ

- **Backend:** ASP.NET Core (C#)
- **Frontend:** HTML, CSS, JavaScript (Vanilla JS)
- **Database:** SQL Server
- **Payment:** Stripe Payment Gateway
- **Authentication:** JWT Token

---

## 2. ACTORS VÀ QUYỀN HẠN

### 2.1. HOCVIEN (Học viên)

**Mô tả:** Người dùng đăng ký tài khoản để học các khóa học trực tuyến.

**Quyền hạn:**

#### A. Quản lý Tài khoản
- Đăng ký tài khoản mới (email, mật khẩu, họ tên)
- Đăng nhập/Đăng xuất
- Xem và chỉnh sửa thông tin cá nhân
- Đổi mật khẩu
- Quên mật khẩu và reset

#### B. Khám phá Khóa học
- Xem danh sách tất cả khóa học công khai (đã được publish và chưa hết hạn)
- Tìm kiếm khóa học theo từ khóa
- Lọc khóa học theo:
  - Danh mục
  - Mức độ (Cơ bản/Trung bình/Nâng cao)
  - Giá (từ thấp đến cao)
  - Đánh giá (từ X sao trở lên)
- Sắp xếp: Mới nhất, Phổ biến, Đánh giá cao, Giá thấp/cao
- Xem các danh sách đặc biệt:
  - Khóa học nổi bật
  - Khóa học bán chạy
  - Khóa học mới nhất
- Xem chi tiết khóa học:
  - Thông tin cơ bản
  - Hình ảnh khóa học
  - Danh sách chương và bài giảng
  - Đánh giá từ học viên khác
  - Thông tin giảng viên

#### C. Đăng ký Khóa học
- Thêm khóa học vào giỏ hàng
- Xem giỏ hàng
- Xóa khóa học khỏi giỏ hàng
- Áp dụng voucher (nếu có)
- Thanh toán qua Stripe
- Xem lịch sử đơn hàng

#### D. Học tập
- Xem danh sách khóa học đã đăng ký
- Học khóa học đã đăng ký:
  - Xem video bài giảng
  - Tải tài liệu bài giảng
  - Đánh dấu bài giảng đã hoàn thành
- **Quy tắc học tập:**
  - Phải hoàn thành bài giảng trước mới được học bài giảng tiếp theo
  - Hoàn thành = xem hết video (hoặc đạt ngưỡng thời gian tối thiểu, ví dụ: 80% thời lượng)
- Xem tiến độ học tập:
  - Số bài giảng đã hoàn thành / Tổng số bài giảng
  - Phần trăm hoàn thành
  - Thời gian học tập
- Tiếp tục học từ bài giảng đang dở
- Nhận thông báo khi giảng viên cập nhật khóa học

#### E. Đánh giá
- Đánh giá khóa học đã đăng ký:
  - Điểm số (1-5 sao)
  - Bình luận (text)
- Chỉnh sửa đánh giá của mình
- Xem đánh giá của học viên khác

#### F. Đăng ký làm Giảng viên
- Gửi yêu cầu đăng ký làm giảng viên:
  - Upload chứng chỉ (ảnh hoặc file PDF)
  - Điền thông tin bổ sung (nếu cần)
- Xem trạng thái yêu cầu (Chờ duyệt/Đã duyệt/Từ chối)
- Nhận thông báo kết quả duyệt

**Hạn chế:**
- Không thể xem khóa học chưa được publish
- Không thể truy cập khóa học đã hết thời hạn (phải gia hạn)
- Không thể đăng ký lại khóa học đã đăng ký (trừ khi đã hết hạn và muốn gia hạn)
- Không thể học bài giảng tiếp theo nếu chưa hoàn thành bài giảng trước

---

### 2.2. GIANGVIEN (Giảng viên)

**Mô tả:** Học viên đã được duyệt và cấp quyền giảng viên, có thể tạo và quản lý khóa học.

**Quyền hạn:**

#### A. Tất cả quyền của HOCVIEN
- Giảng viên có thể làm tất cả những gì học viên làm

#### B. Quản lý Khóa học

**Tạo Khóa học:**
- Tạo khóa học mới với các thông tin:
  - Tên khóa học (bắt buộc, tối đa 255 ký tự)
  - Mô tả ngắn (bắt buộc, tối đa 500 ký tự)
  - Mô tả chi tiết (optional, text)
  - Danh mục (bắt buộc, chọn từ danh sách)
  - Giá bán (bắt buộc, >= 0)
  - Hình đại diện (bắt buộc, upload ảnh)
  - Mức độ (bắt buộc: Cơ bản/Trung bình/Nâng cao)
  - Yêu cầu trước khi học (optional, text)
  - Những gì học được (optional, text)
- Tạo chương (Chuong):
  - Tên chương (bắt buộc, tối đa 255 ký tự)
  - Mô tả chương (optional, tối đa 500 ký tự)
  - Thứ tự chương (bắt buộc, số nguyên dương)
- Tạo bài giảng (BaiGiang) trong mỗi chương:
  - Tiêu đề bài giảng (bắt buộc, tối đa 200 ký tự)
  - Mô tả bài giảng (optional, tối đa 500 ký tự)
  - Upload video bài giảng (bắt buộc)
  - Upload tài liệu bài giảng (optional, nhiều file)
  - Thời lượng bài giảng (tính bằng giây, tự động từ video hoặc nhập thủ công)
  - Thứ tự bài giảng trong chương (bắt buộc, số nguyên dương)
  - Đánh dấu bài giảng xem thử miễn phí (optional, boolean)
- Lưu nháp hoặc gửi để publish
- Khi gửi để publish, khóa học tự động được publish (không cần duyệt)

**Chỉnh sửa Khóa học:**
- **Có thể chỉnh sửa bất cứ lúc nào**, kể cả khi đã có học viên đăng ký
- Chỉnh sửa thông tin cơ bản
- Thêm/Xóa/Sửa chương
- Thêm/Xóa/Sửa bài giảng trong chương
- Upload video/tài liệu mới
- **Khi chỉnh sửa khóa học đã có học viên đăng ký:**
  - Phải gửi thông báo đến tất cả học viên đã đăng ký khóa học đó
  - Thông báo đơn giản: "Khóa học [Tên khóa học] đã được cập nhật"
  - Thông báo được gửi qua hệ thống (in-app notification)
  - Học viên nhận thông báo trong trang "Thông báo" hoặc dashboard

**Xóa Khóa học:**
- **Có thể xóa khi:**
  - Khóa học chưa có học viên đăng ký
  - Khóa học chưa được publish
- **Không thể xóa khi:**
  - Khóa học đã có học viên đăng ký
  - Khóa học đang được học viên học

**Quản lý Khóa học:**
- Xem danh sách tất cả khóa học của mình
- Xem chi tiết khóa học
- Xem số lượng học viên đã đăng ký
- Xem tiến độ học tập của học viên (tổng hợp)

#### C. Dashboard Giảng viên
- Tổng số khóa học
- Tổng số học viên
- Tổng doanh thu (70% từ mỗi khóa học)
- Đánh giá trung bình
- Danh sách khóa học gần đây
- Thống kê theo thời gian

#### D. Quản lý Học viên
- Xem danh sách học viên đã đăng ký khóa học của mình
- Xem tiến độ học tập của từng học viên:
  - Số bài giảng đã hoàn thành
  - Phần trăm hoàn thành
  - Thời gian học tập

**Hạn chế:**
- Không thể xóa khóa học đã có học viên đăng ký
- Không thể chỉnh sửa thông tin thanh toán sau khi đã có học viên đăng ký (giá, voucher)

---

### 2.3. KIEMDUYETVIEN (Kiểm duyệt viên)

**Mô tả:** Người dùng chỉ có quyền kiểm duyệt yêu cầu đăng ký làm giảng viên.

**Quyền hạn:**

#### A. Kiểm duyệt Yêu cầu Đăng ký làm Giảng viên
- Xem danh sách yêu cầu đăng ký làm giảng viên:
  - Tất cả yêu cầu
  - Chỉ yêu cầu chờ duyệt
  - Yêu cầu đã duyệt
  - Yêu cầu bị từ chối
- Xem chi tiết yêu cầu:
  - Thông tin học viên (họ tên, email, ngày đăng ký)
  - Chứng chỉ đã upload (xem ảnh/file PDF)
  - Thông tin bổ sung (nếu có)
  - Ngày gửi yêu cầu
- Duyệt yêu cầu:
  - Phê duyệt → Cấp role GIANGVIEN cho học viên
  - Từ chối → Nhập lý do từ chối
- Xem lịch sử kiểm duyệt

**Hạn chế:**
- **KHÔNG có quyền của HOCVIEN:**
  - Không thể xem danh sách khóa học
  - Không thể đăng ký khóa học
  - Không thể học tập
  - Không thể đánh giá
- **CHỈ có thể truy cập:**
  - Trang đăng nhập
  - Trang kiểm duyệt (dashboard kiểm duyệt)
  - Trang xem chi tiết yêu cầu
- **KHÔNG có quyền:**
  - Kiểm duyệt khóa học (khóa học tự động publish khi giảng viên tạo)
  - Quản lý người dùng
  - Quản lý danh mục
  - Xem báo cáo tổng hợp

---

### 2.4. ADMIN (Quản trị viên hệ thống)

**Mô tả:** Người dùng có quyền quản lý toàn bộ hệ thống.

**Quyền hạn:**

#### A. Tất cả quyền của KIEMDUYETVIEN
- Kiểm duyệt yêu cầu đăng ký làm giảng viên

#### B. Quản lý Người dùng
- Xem danh sách tất cả người dùng
- Xem chi tiết người dùng
- Chỉnh sửa thông tin người dùng
- Vô hiệu hóa/Kích hoạt tài khoản
- Gán/Xóa vai trò cho người dùng
- Xem lịch sử hoạt động của người dùng

#### C. Quản lý Danh mục
- Tạo danh mục mới
- Chỉnh sửa danh mục
- Xóa danh mục
- Xem danh sách danh mục

#### D. Quản lý Khóa học
- Xem danh sách tất cả khóa học
- Xem chi tiết khóa học
- Ẩn/Bỏ ẩn khóa học
- Xóa khóa học (nếu cần thiết)

#### E. Quản lý Đơn hàng
- Xem danh sách tất cả đơn hàng
- Xem chi tiết đơn hàng
- Xử lý đơn hàng (nếu có vấn đề)

#### F. Báo cáo và Thống kê
- Tổng số người dùng
- Tổng số khóa học
- Tổng số học viên
- Tổng doanh thu (30% từ mỗi khóa học)
- Thống kê theo thời gian
- Thống kê theo danh mục
- Thống kê theo giảng viên

---

## 3. USE CASES CHI TIẾT

### 3.1. UC-001: Đăng ký Tài khoản

**Actor:** HOCVIEN (chưa đăng nhập)

**Mô tả:** Người dùng đăng ký tài khoản mới để sử dụng hệ thống.

**Preconditions:**
- Người dùng chưa có tài khoản
- Đang ở trang đăng ký

**Main Flow:**
1. Người dùng điền thông tin:
   - Họ tên (bắt buộc)
   - Email (bắt buộc, định dạng email hợp lệ)
   - Mật khẩu (bắt buộc, tối thiểu 6 ký tự)
   - Xác nhận mật khẩu (bắt buộc, phải khớp với mật khẩu)
2. Hệ thống validate thông tin
3. Hệ thống kiểm tra email đã tồn tại chưa
4. Nếu email chưa tồn tại:
   - Tạo tài khoản mới với role HOCVIEN
   - Hash mật khẩu
   - Lưu vào database
   - Trả về thông báo thành công
5. Người dùng được chuyển đến trang đăng nhập

**Alternative Flows:**
- 3a. Email đã tồn tại:
  - Hiển thị lỗi "Email đã được sử dụng"
  - Người dùng nhập email khác
- 3b. Mật khẩu không đủ mạnh:
  - Hiển thị lỗi "Mật khẩu phải có ít nhất 6 ký tự"
  - Người dùng nhập lại mật khẩu
- 3c. Xác nhận mật khẩu không khớp:
  - Hiển thị lỗi "Mật khẩu xác nhận không khớp"
  - Người dùng nhập lại

**Postconditions:**
- Tài khoản mới được tạo với role HOCVIEN
- Người dùng có thể đăng nhập

---

### 3.2. UC-002: Đăng nhập

**Actor:** Tất cả người dùng

**Mô tả:** Người dùng đăng nhập vào hệ thống.

**Preconditions:**
- Người dùng đã có tài khoản
- Đang ở trang đăng nhập

**Main Flow:**
1. Người dùng nhập email và mật khẩu
2. Hệ thống validate thông tin
3. Hệ thống kiểm tra email và mật khẩu
4. Nếu đúng:
   - Tạo JWT token
   - Lưu refresh token
   - Trả về access token và refresh token
5. Người dùng được chuyển đến trang chủ (theo role):
   - HOCVIEN → Trang chủ học viên
   - GIANGVIEN → Dashboard giảng viên
   - KIEMDUYETVIEN → Trang kiểm duyệt
   - ADMIN → Dashboard admin

**Alternative Flows:**
- 3a. Email hoặc mật khẩu sai:
  - Hiển thị lỗi "Email hoặc mật khẩu không đúng"
  - Người dùng nhập lại
- 3b. Tài khoản bị vô hiệu hóa:
  - Hiển thị lỗi "Tài khoản đã bị vô hiệu hóa"
  - Người dùng liên hệ admin

**Postconditions:**
- Người dùng đã đăng nhập
- Token được lưu trong localStorage
- Người dùng có thể truy cập các trang theo quyền

---

### 3.3. UC-003: Đăng ký làm Giảng viên

**Actor:** HOCVIEN

**Mô tả:** Học viên gửi yêu cầu đăng ký làm giảng viên bằng cách upload chứng chỉ.

**Preconditions:**
- Người dùng đã đăng nhập với role HOCVIEN
- Người dùng chưa có role GIANGVIEN

**Main Flow:**
1. Người dùng vào trang "Đăng ký làm giảng viên"
2. Người dùng upload chứng chỉ:
   - File ảnh (JPG, PNG) hoặc PDF
   - Kích thước tối đa: 10MB
3. Người dùng điền thông tin bổ sung (optional):
   - Kinh nghiệm giảng dạy
   - Chuyên môn
   - Giới thiệu bản thân
4. Người dùng nhấn "Gửi yêu cầu"
5. Hệ thống validate:
   - File chứng chỉ phải có
   - File phải đúng định dạng và kích thước
6. Hệ thống lưu file chứng chỉ
7. Hệ thống tạo bản ghi yêu cầu với trạng thái "Chờ duyệt"
8. Hệ thống thông báo thành công
9. Người dùng được chuyển đến trang xem trạng thái yêu cầu

**Alternative Flows:**
- 5a. File không hợp lệ:
  - Hiển thị lỗi "File không hợp lệ. Vui lòng upload file ảnh hoặc PDF"
  - Người dùng upload lại
- 5b. File quá lớn:
  - Hiển thị lỗi "File quá lớn. Kích thước tối đa 10MB"
  - Người dùng upload file nhỏ hơn

**Postconditions:**
- Yêu cầu được tạo với trạng thái "Chờ duyệt"
- KIEMDUYETVIEN/ADMIN có thể xem và duyệt yêu cầu

---

### 3.4. UC-004: Duyệt Yêu cầu Đăng ký làm Giảng viên

**Actor:** KIEMDUYETVIEN hoặc ADMIN

**Mô tả:** Kiểm duyệt viên hoặc Admin xem xét và duyệt/từ chối yêu cầu đăng ký làm giảng viên.

**Preconditions:**
- Người dùng đã đăng nhập với role KIEMDUYETVIEN hoặc ADMIN
- Có yêu cầu chờ duyệt

**Main Flow:**
1. Người dùng vào trang "Kiểm duyệt"
2. Hệ thống hiển thị danh sách yêu cầu chờ duyệt
3. Người dùng chọn một yêu cầu để xem chi tiết
4. Hệ thống hiển thị:
   - Thông tin học viên (họ tên, email, ngày đăng ký)
   - Chứng chỉ đã upload (xem ảnh/file PDF)
   - Thông tin bổ sung (nếu có)
   - Ngày gửi yêu cầu
5. Người dùng xem xét và quyết định:
   - **Duyệt:**
     a. Người dùng nhấn "Duyệt"
     b. Hệ thống cập nhật trạng thái yêu cầu thành "Đã duyệt"
     c. Hệ thống thêm role GIANGVIEN cho học viên
     d. Hệ thống gửi thông báo cho học viên: "Yêu cầu đăng ký làm giảng viên của bạn đã được duyệt"
     e. Hệ thống thông báo thành công
   - **Từ chối:**
     a. Người dùng nhấn "Từ chối"
     b. Hệ thống hiển thị form nhập lý do từ chối
     c. Người dùng nhập lý do từ chối (bắt buộc)
     d. Người dùng nhấn "Xác nhận từ chối"
     e. Hệ thống cập nhật trạng thái yêu cầu thành "Từ chối"
     f. Hệ thống lưu lý do từ chối
     g. Hệ thống gửi thông báo cho học viên: "Yêu cầu đăng ký làm giảng viên của bạn đã bị từ chối. Lý do: [Lý do]"
     h. Hệ thống thông báo thành công
6. Người dùng quay lại danh sách yêu cầu

**Alternative Flows:**
- 5b. Người dùng không nhập lý do từ chối:
  - Hiển thị lỗi "Vui lòng nhập lý do từ chối"
  - Người dùng nhập lý do

**Postconditions:**
- Yêu cầu được cập nhật trạng thái
- Nếu duyệt: Học viên có thêm role GIANGVIEN
- Học viên nhận được thông báo kết quả

---

### 3.5. UC-005: Tạo Khóa học

**Actor:** GIANGVIEN

**Mô tả:** Giảng viên tạo khóa học mới với đầy đủ thông tin và nội dung.

**Preconditions:**
- Người dùng đã đăng nhập với role GIANGVIEN
- Người dùng vào trang "Tạo khóa học"

**Main Flow:**
1. Người dùng điền thông tin cơ bản:
   - Tên khóa học (bắt buộc)
   - Mô tả ngắn (bắt buộc)
   - Mô tả chi tiết (optional)
   - Chọn danh mục (bắt buộc)
   - Nhập giá bán (bắt buộc, >= 0)
   - Upload hình đại diện (bắt buộc)
   - Chọn mức độ (bắt buộc)
   - Nhập yêu cầu trước khi học (optional)
   - Nhập những gì học được (optional)
2. Người dùng tạo chương:
   - Nhấn "Thêm chương"
   - Nhập tên chương (bắt buộc)
   - Nhập mô tả chương (optional)
   - Thiết lập thứ tự chương
3. Người dùng tạo bài giảng trong chương:
   - Nhấn "Thêm bài giảng" trong chương
   - Nhập tiêu đề bài giảng (bắt buộc)
   - Nhập mô tả bài giảng (optional)
   - Upload video bài giảng (bắt buộc)
   - Upload tài liệu bài giảng (optional, nhiều file)
   - Hệ thống tự động tính thời lượng từ video (hoặc nhập thủ công)
   - Thiết lập thứ tự bài giảng
   - Đánh dấu bài giảng xem thử miễn phí (optional)
4. Người dùng có thể:
   - Lưu nháp (chưa publish)
   - Gửi để publish (tự động publish, không cần duyệt)
5. Nếu chọn "Gửi để publish":
   - Hệ thống validate:
     - Phải có ít nhất 1 chương
     - Mỗi chương phải có ít nhất 1 bài giảng
     - Mỗi bài giảng phải có video
   - Nếu hợp lệ:
     - Lưu khóa học với trạng thái "Đã publish"
     - Thiết lập NgayPublish = ngày hiện tại
     - Hiển thị công khai trên trang web
     - Thông báo thành công
   - Nếu không hợp lệ:
     - Hiển thị lỗi validation
     - Người dùng chỉnh sửa và gửi lại

**Alternative Flows:**
- 5a. Không có chương nào:
  - Hiển thị lỗi "Khóa học phải có ít nhất 1 chương"
- 5b. Chương không có bài giảng:
  - Hiển thị lỗi "Chương '[Tên chương]' phải có ít nhất 1 bài giảng"
- 5c. Bài giảng không có video:
  - Hiển thị lỗi "Bài giảng '[Tên bài giảng]' phải có video"

**Postconditions:**
- Khóa học được tạo và publish
- Khóa học hiển thị công khai trên trang web
- Khóa học tồn tại vĩnh viễn trong hệ thống

---

### 3.6. UC-006: Chỉnh sửa Khóa học

**Actor:** GIANGVIEN

**Mô tả:** Giảng viên chỉnh sửa khóa học của mình, kể cả khi đã có học viên đăng ký.

**Preconditions:**
- Người dùng đã đăng nhập với role GIANGVIEN
- Khóa học thuộc về giảng viên này
- Khóa học chưa bị xóa

**Main Flow:**
1. Người dùng vào trang "Quản lý khóa học"
2. Người dùng chọn khóa học cần chỉnh sửa
3. Người dùng nhấn "Chỉnh sửa"
4. Hệ thống hiển thị form chỉnh sửa với thông tin hiện tại
5. Người dùng chỉnh sửa:
   - Thông tin cơ bản
   - Thêm/Xóa/Sửa chương
   - Thêm/Xóa/Sửa bài giảng
   - Upload video/tài liệu mới
6. Người dùng nhấn "Lưu thay đổi"
7. Hệ thống validate thông tin
8. Nếu hợp lệ:
   - Lưu thay đổi vào database
   - **Nếu khóa học đã có học viên đăng ký:**
     a. Hệ thống lấy danh sách tất cả học viên đã đăng ký khóa học này
     b. Hệ thống tạo thông báo cho mỗi học viên:
        - Nội dung: "Khóa học '[Tên khóa học]' đã được cập nhật"
        - Loại: "Khóa học cập nhật"
        - Trạng thái: "Chưa đọc"
        - Ngày tạo: Ngày hiện tại
     c. Hệ thống lưu thông báo vào bảng Notifications
   - Thông báo thành công
9. Người dùng quay lại trang quản lý khóa học

**Alternative Flows:**
- 7a. Validation lỗi:
  - Hiển thị lỗi validation
  - Người dùng chỉnh sửa và lưu lại

**Postconditions:**
- Khóa học được cập nhật
- Nếu có học viên đăng ký: Tất cả học viên nhận được thông báo cập nhật

---

### 3.7. UC-007: Đăng ký Khóa học

**Actor:** HOCVIEN

**Mô tả:** Học viên đăng ký khóa học bằng cách thanh toán.

**Preconditions:**
- Người dùng đã đăng nhập với role HOCVIEN
- Khóa học đã được publish
- Người dùng chưa đăng ký khóa học này hoặc đã hết thời hạn truy cập

**Main Flow:**
1. Người dùng xem chi tiết khóa học
2. Người dùng nhấn "Thêm vào giỏ hàng"
3. Hệ thống thêm khóa học vào giỏ hàng của người dùng
4. Người dùng vào trang "Giỏ hàng"
5. Người dùng có thể:
   - Áp dụng voucher (nếu có)
   - Xóa khóa học khỏi giỏ hàng
6. Người dùng nhấn "Thanh toán"
7. Hệ thống chuyển đến trang thanh toán (Stripe)
8. Người dùng nhập thông tin thanh toán:
   - Số thẻ
   - Ngày hết hạn
   - CVV
   - Tên chủ thẻ
9. Người dùng nhấn "Thanh toán"
10. Hệ thống gọi Stripe API để xử lý thanh toán
11. Nếu thanh toán thành công:
    a. Hệ thống tạo đơn hàng (DonHang):
       - IdNguoiDung
       - IdVoucher (nếu có)
       - TongTienGoc
       - TienGiam (từ voucher)
       - TongTienThanhToan
       - TrangThaiThanhToan = "Đã thanh toán"
       - TrangThaiDonHang = "Hoàn thành"
       - StripePaymentIntentId
    b. Hệ thống tạo chi tiết đơn hàng (ChiTietDonHang)
    c. Hệ thống đăng ký khóa học cho học viên (DangKyKhoaHoc):
       - IdHocVien
       - IdKhoaHoc
       - IdDonHang
       - NgayDangKy = ngày hiện tại
       - NgayHetHan = NgayDangKy + thời hạn truy cập (ví dụ: 6 tháng hoặc 12 tháng)
       - TrangThai = true
    d. Hệ thống tạo tiến độ học tập (TienDoHocTap):
       - IdDangKyKhoaHoc
       - IdKhoaHoc
       - IdHocVien
       - SoBaiHocDaHoanThanh = 0
       - TongSoBaiHoc = tổng số bài giảng của khóa học
       - PhanTramHoanThanh = 0
       - DaHoanThanh = false
       - NgayBatDau = ngày hiện tại
    e. Hệ thống tính chia sẻ doanh thu:
       - TienGiangVien = TongTienThanhToan * 0.7
       - TienAdmin = TongTienThanhToan * 0.3
    f. Hệ thống tạo bản ghi chia sẻ doanh thu (ChiTietChiaSeDoanhThu)
    g. Hệ thống xóa khóa học khỏi giỏ hàng
    h. Hệ thống thông báo thành công
    i. Người dùng được chuyển đến trang "Khóa học của tôi"
12. Nếu thanh toán thất bại:
    a. Hệ thống hiển thị lỗi từ Stripe
    b. Người dùng có thể thử lại

**Alternative Flows:**
- 2b. Người dùng đã đăng ký khóa học này và chưa hết hạn:
  - Hiển thị thông báo "Bạn đã đăng ký khóa học này"
  - Không thể thêm vào giỏ hàng
- 2c. Người dùng đã đăng ký nhưng đã hết thời hạn:
  - Hiển thị thông báo "Thời hạn truy cập đã hết hạn. Vui lòng gia hạn để tiếp tục học"
  - Có thể thêm vào giỏ hàng để gia hạn
- 10a. Thanh toán thất bại:
  - Hiển thị lỗi từ Stripe
  - Người dùng có thể thử lại hoặc hủy

**Postconditions:**
- Đơn hàng được tạo
- Học viên đã đăng ký khóa học
- Tiến độ học tập được tạo
- Doanh thu được chia sẻ

---

### 3.8. UC-008: Học Khóa học

**Actor:** HOCVIEN

**Mô tả:** Học viên học khóa học đã đăng ký, học tuần tự từng bài giảng.

**Preconditions:**
- Người dùng đã đăng nhập với role HOCVIEN
- Người dùng đã đăng ký khóa học này
- Thời hạn truy cập khóa học chưa hết hạn
- Khóa học vẫn tồn tại (chưa bị xóa)

**Main Flow:**
1. Người dùng vào trang "Khóa học của tôi"
2. Người dùng chọn khóa học cần học
3. Hệ thống hiển thị:
   - Danh sách chương và bài giảng
   - Bài giảng đã hoàn thành (có dấu tích)
   - Bài giảng đang học (highlight)
   - Bài giảng chưa mở khóa (màu xám, không click được)
4. Người dùng chọn bài giảng để học:
   - **Nếu là bài giảng đầu tiên hoặc bài giảng trước đã hoàn thành:**
     a. Hệ thống cho phép học
     b. Hệ thống hiển thị:
        - Video bài giảng với player
        - Tài liệu bài giảng (nếu có, có thể tải xuống)
        - Mô tả bài giảng
     c. Người dùng xem video
     d. Hệ thống theo dõi thời gian xem:
        - Ghi nhận thời gian bắt đầu xem
        - Ghi nhận thời gian xem video
        - Tính phần trăm đã xem
     e. Khi người dùng xem hết video (hoặc đạt 80% thời lượng):
        - Hệ thống tự động đánh dấu "Đã xem hết video"
     f. Người dùng nhấn "Hoàn thành bài giảng"
     g. Hệ thống validate:
        - Đã xem hết video (hoặc đạt 80% thời lượng)
     h. Nếu hợp lệ:
        - Hệ thống cập nhật TienDoHocTapChiTiet:
          - IdBaiGiang
          - DaHoanThanh = true
          - ThoiGianBatDauHoc = thời gian bắt đầu xem
          - ThoiGianHoanThanh = thời gian hiện tại
          - DaXemHetVideo = true
        - Hệ thống cập nhật TienDoHocTap:
          - SoBaiHocDaHoanThanh += 1
          - PhanTramHoanThanh = (SoBaiHocDaHoanThanh / TongSoBaiHoc) * 100
          - Nếu SoBaiHocDaHoanThanh == TongSoBaiHoc:
            - DaHoanThanh = true
            - NgayHoanThanh = ngày hiện tại
        - Hệ thống mở khóa bài giảng tiếp theo
        - Thông báo thành công
   - **Nếu là bài giảng chưa mở khóa:**
     a. Hệ thống không cho phép học
     b. Hiển thị thông báo "Vui lòng hoàn thành bài giảng trước đó"

**Alternative Flows:**
- 4g. Chưa xem hết video:
  - Hiển thị thông báo "Vui lòng xem hết video để hoàn thành bài giảng"
  - Người dùng tiếp tục xem video

**Postconditions:**
- Bài giảng được đánh dấu hoàn thành
- Bài giảng tiếp theo được mở khóa
- Tiến độ học tập được cập nhật

---

### 3.9. UC-009: Quản lý Thời hạn Truy cập của Học viên

**Actor:** Hệ thống (Background Job)

**Mô tả:** Hệ thống tự động quản lý thời hạn truy cập khóa học của học viên.

**Preconditions:**
- Có học viên đã đăng ký khóa học

**Main Flow:**
1. Hệ thống chạy background job định kỳ (mỗi ngày)
2. Hệ thống lấy danh sách đăng ký khóa học đã hết thời hạn (NgayHetHan < ngày hiện tại)
3. Với mỗi đăng ký hết hạn:
   a. Hệ thống cập nhật trạng thái:
      - Cập nhật DangKyKhoaHoc.TrangThai = false (hết hạn)
      - Học viên không thể truy cập khóa học nữa
   b. Hệ thống gửi thông báo cho học viên:
      - Nội dung: "Thời hạn truy cập khóa học '[Tên khóa học]' đã hết hạn. Vui lòng gia hạn để tiếp tục học"
      - Loại: "Khóa học hết hạn"
   c. Học viên có thể gia hạn bằng cách thanh toán lại khóa học

**Postconditions:**
- Đăng ký hết hạn được cập nhật trạng thái
- Học viên nhận được thông báo hết hạn
- Học viên có thể gia hạn để tiếp tục học

---

## 4. QUY TRÌNH NGHIỆP VỤ

### 4.1. Quy trình Đăng ký làm Giảng viên

```
[HOCVIEN đăng nhập]
    ↓
Vào trang "Đăng ký làm giảng viên"
    ↓
Upload chứng chỉ (ảnh/PDF)
    ↓
Điền thông tin bổ sung (optional)
    ↓
Gửi yêu cầu
    ↓
[Hệ thống tạo YeuCauDangKyGiangVien với TrangThai = "Chờ duyệt"]
    ↓
[KIEMDUYETVIEN/ADMIN đăng nhập]
    ↓
Vào trang "Kiểm duyệt"
    ↓
Xem danh sách yêu cầu chờ duyệt
    ↓
Chọn yêu cầu → Xem chi tiết
    ↓
Xem chứng chỉ và thông tin
    ↓
Quyết định:
    ├─→ Duyệt
    │   ↓
    │   Cập nhật TrangThai = "Đã duyệt"
    │   ↓
    │   Thêm role GIANGVIEN cho học viên
    │   ↓
    │   Gửi thông báo cho học viên: "Yêu cầu đã được duyệt"
    │   ↓
    │   [Học viên có quyền GIANGVIEN]
    │
    └─→ Từ chối
        ↓
        Nhập lý do từ chối
        ↓
        Cập nhật TrangThai = "Từ chối"
        ↓
        Lưu lý do từ chối
        ↓
        Gửi thông báo cho học viên: "Yêu cầu bị từ chối. Lý do: [Lý do]"
```

### 4.2. Quy trình Tạo và Publish Khóa học

```
[GIANGVIEN đăng nhập]
    ↓
Vào trang "Tạo khóa học"
    ↓
Điền thông tin cơ bản
    ↓
Tạo chương và bài giảng
    ↓
Upload video và tài liệu
    ↓
Nhấn "Gửi để publish"
    ↓
[Hệ thống validate]
    ├─→ Không hợp lệ → Hiển thị lỗi → Chỉnh sửa
    └─→ Hợp lệ
        ↓
Lưu khóa học với TrangThai = "Đã publish"
    ↓
Thiết lập NgayPublish = ngày hiện tại
    ↓
Hiển thị công khai trên trang web
    ↓
[Khóa học tồn tại vĩnh viễn trong hệ thống]
```

### 4.3. Quy trình Chỉnh sửa Khóa học có Học viên

```
[GIANGVIEN đăng nhập]
    ↓
Vào trang "Quản lý khóa học"
    ↓
Chọn khóa học cần chỉnh sửa
    ↓
Nhấn "Chỉnh sửa"
    ↓
Chỉnh sửa thông tin/nội dung
    ↓
Nhấn "Lưu thay đổi"
    ↓
[Hệ thống kiểm tra]
    ├─→ Khóa học chưa có học viên đăng ký
    │   ↓
    │   Lưu thay đổi
    │   ↓
    │   Thông báo thành công
    │
    └─→ Khóa học đã có học viên đăng ký
        ↓
        Lưu thay đổi
        ↓
        Lấy danh sách học viên đã đăng ký
        ↓
        Với mỗi học viên:
            Tạo thông báo:
            - NoiDung: "Khóa học '[Tên khóa học]' đã được cập nhật"
            - Loai: "Khóa học cập nhật"
            - TrangThai: "Chưa đọc"
            - NgayTao: ngày hiện tại
        ↓
        Lưu thông báo vào database
        ↓
        Thông báo thành công
        ↓
        [Học viên nhận thông báo khi đăng nhập]
```

### 4.4. Quy trình Đăng ký và Học tập

```
[HOCVIEN đăng nhập]
    ↓
Xem danh sách khóa học
    ↓
Chọn khóa học → Xem chi tiết
    ↓
Nhấn "Thêm vào giỏ hàng"
    ↓
Vào trang "Giỏ hàng"
    ↓
Áp dụng voucher (nếu có)
    ↓
Nhấn "Thanh toán"
    ↓
Chuyển đến trang thanh toán Stripe
    ↓
Nhập thông tin thanh toán
    ↓
Nhấn "Thanh toán"
    ↓
[Stripe xử lý thanh toán]
    ├─→ Thất bại → Hiển thị lỗi → Thử lại
    └─→ Thành công
        ↓
Tạo đơn hàng (DonHang)
    ↓
Tạo chi tiết đơn hàng (ChiTietDonHang)
    ↓
Đăng ký khóa học (DangKyKhoaHoc)
    ↓
Tạo tiến độ học tập (TienDoHocTap)
    ↓
Tính chia sẻ doanh thu (70% giảng viên, 30% admin)
    ↓
Tạo bản ghi chia sẻ doanh thu
    ↓
Xóa khóa học khỏi giỏ hàng
    ↓
Chuyển đến trang "Khóa học của tôi"
    ↓
[Học viên có thể bắt đầu học]
    ↓
Học bài giảng đầu tiên
    ↓
Xem video → Hoàn thành bài giảng
    ↓
[Mở khóa bài giảng tiếp theo]
    ↓
Tiếp tục học tuần tự
    ↓
Hoàn thành tất cả bài giảng
    ↓
[Đánh giá khóa học (optional)]
```

### 4.5. Quy trình Quản lý Thời hạn Truy cập của Học viên

```
[Background Job chạy mỗi ngày]
    ↓
Lấy danh sách đăng ký khóa học đã hết thời hạn
(NgayHetHan < ngày hiện tại và TrangThai = true)
    ↓
Với mỗi đăng ký hết hạn:
    ↓
Cập nhật TrangThai = false
    ↓
Gửi thông báo cho học viên về việc hết hạn
    ↓
[Học viên không thể truy cập khóa học]
    ↓
Học viên có thể gia hạn bằng cách thanh toán lại
    ↓
Khi gia hạn: Tạo đăng ký mới với NgayHetHan mới
```

---

## 5. CẤU TRÚC DATABASE

### 5.1. Bảng Mới Cần Tạo

#### **A. YeuCauDangKyGiangVien (Yêu cầu đăng ký làm giảng viên)**

```sql
CREATE TABLE YeuCauDangKyGiangVien (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdHocVien INT NOT NULL,
    ChungChiPath NVARCHAR(500) NOT NULL, -- Đường dẫn file chứng chỉ
    ThongTinBoSung NVARCHAR(MAX), -- Thông tin bổ sung (JSON hoặc text)
    TrangThai NVARCHAR(50) NOT NULL, -- Chờ duyệt/Đã duyệt/Từ chối
    LyDoTuChoi NVARCHAR(500), -- Lý do từ chối (nếu bị từ chối)
    NgayGui DATETIME NOT NULL DEFAULT GETDATE(),
    NgayDuyet DATETIME, -- Ngày duyệt/từ chối
    IdNguoiDuyet INT, -- ID người duyệt (KIEMDUYETVIEN hoặc ADMIN)
    
    FOREIGN KEY (IdHocVien) REFERENCES NguoiDung(Id),
    FOREIGN KEY (IdNguoiDuyet) REFERENCES NguoiDung(Id)
);
```

#### **B. Notification (Thông báo)**

```sql
CREATE TABLE Notification (
    Id INT PRIMARY KEY IDENTITY(1,1),
    IdNguoiDung INT NOT NULL, -- Người nhận thông báo
    TieuDe NVARCHAR(255) NOT NULL, -- Tiêu đề thông báo
    NoiDung NVARCHAR(MAX) NOT NULL, -- Nội dung thông báo
    Loai NVARCHAR(50) NOT NULL, -- Loại thông báo (Khóa học cập nhật, Yêu cầu duyệt, etc.)
    TrangThai NVARCHAR(50) NOT NULL DEFAULT 'Chưa đọc', -- Chưa đọc/Đã đọc
    IdKhoaHoc INT, -- ID khóa học liên quan (nếu có)
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    NgayDoc DATETIME, -- Ngày đọc thông báo
    
    FOREIGN KEY (IdNguoiDung) REFERENCES NguoiDung(Id),
    FOREIGN KEY (IdKhoaHoc) REFERENCES KhoaHoc(Id)
);
```

### 5.2. Bảng Cần Chỉnh sửa

#### **A. KhoaHoc**

**Thêm các cột:**
```sql
ALTER TABLE KhoaHoc
ADD NgayPublish DATETIME; -- Ngày được publish
```

**Lưu ý:**
- Khóa học không có thời hạn tự động hết hạn
- Khóa học tồn tại vĩnh viễn trong hệ thống sau khi được publish
- Có thể giữ lại bảng KiemDuyetKhoaHoc để lưu lịch sử kiểm duyệt (không bắt buộc)

#### **B. DangKyKhoaHoc**

**Thêm các cột:**
```sql
ALTER TABLE DangKyKhoaHoc
ADD NgayHetHan DATETIME; -- Ngày hết thời hạn truy cập của học viên (NgayDangKy + thời hạn, ví dụ: 6 tháng hoặc 12 tháng)
```

#### **C. TienDoHocTapChiTiet**

**Thêm các cột:**
```sql
ALTER TABLE TienDoHocTapChiTiet
ADD ThoiGianBatDauHoc DATETIME, -- Thời gian bắt đầu học bài giảng
    ThoiGianHoanThanh DATETIME, -- Thời gian hoàn thành bài giảng
    DaXemHetVideo BIT DEFAULT 0, -- Đã xem hết video chưa
    PhanTramDaXem DECIMAL(5,2) DEFAULT 0; -- Phần trăm đã xem (để tính 80%)
```

### 5.3. Bảng Giữ Nguyên

- NguoiDung
- VaiTro
- NguoiDungVaiTro
- DanhMucKhoaHoc
- Chuong
- BaiGiang
- TaiLieuBaiGiang
- DangKyKhoaHoc
- TienDoHocTap
- DonHang
- ChiTietDonHang
- GioHang
- ChiTietGioHang
- DanhGiaKhoaHoc
- Voucher
- ChiaSeLuanNhuan
- ChiTietChiaSeDoanhThu

---

## 6. API SPECIFICATIONS

### 6.1. Authentication APIs

#### **POST /api/v1/auth/register**
Đăng ký tài khoản mới

**Request:**
```json
{
  "hoTen": "string",
  "email": "string",
  "matKhau": "string",
  "xacNhanMatKhau": "string"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Đăng ký thành công",
  "data": {
    "id": 1,
    "email": "user@example.com"
  }
}
```

#### **POST /api/v1/auth/login**
Đăng nhập

**Request:**
```json
{
  "email": "string",
  "matKhau": "string"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "accessToken": "string",
    "refreshToken": "string",
    "user": {
      "id": 1,
      "email": "string",
      "hoTen": "string",
      "roles": ["HOCVIEN"]
    }
  }
}
```

### 6.2. Instructor Registration APIs

#### **POST /api/v1/instructor-requests**
Gửi yêu cầu đăng ký làm giảng viên

**Authorization:** Required (HOCVIEN)

**Request:** FormData
- chungChi: File (ảnh hoặc PDF)
- thongTinBoSung: string (optional)

**Response:**
```json
{
  "success": true,
  "message": "Yêu cầu đã được gửi",
  "data": {
    "id": 1,
    "trangThai": "Chờ duyệt"
  }
}
```

#### **GET /api/v1/instructor-requests**
Lấy danh sách yêu cầu đăng ký làm giảng viên

**Authorization:** Required (KIEMDUYETVIEN hoặc ADMIN)

**Query Parameters:**
- trangThai: string (optional) - Chờ duyệt/Đã duyệt/Từ chối
- pageNumber: int (default: 1)
- pageSize: int (default: 10)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "idHocVien": 1,
        "hoTen": "string",
        "email": "string",
        "chungChiPath": "string",
        "trangThai": "Chờ duyệt",
        "ngayGui": "2024-01-01T00:00:00"
      }
    ],
    "totalCount": 10,
    "pageNumber": 1,
    "pageSize": 10
  }
}
```

#### **GET /api/v1/instructor-requests/{id}**
Lấy chi tiết yêu cầu đăng ký làm giảng viên

**Authorization:** Required (KIEMDUYETVIEN hoặc ADMIN)

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "idHocVien": 1,
    "hoTen": "string",
    "email": "string",
    "chungChiPath": "string",
    "thongTinBoSung": "string",
    "trangThai": "Chờ duyệt",
    "lyDoTuChoi": null,
    "ngayGui": "2024-01-01T00:00:00",
    "ngayDuyet": null,
    "idNguoiDuyet": null
  }
}
```

#### **POST /api/v1/instructor-requests/{id}/approve**
Duyệt yêu cầu đăng ký làm giảng viên

**Authorization:** Required (KIEMDUYETVIEN hoặc ADMIN)

**Response:**
```json
{
  "success": true,
  "message": "Đã duyệt yêu cầu thành công"
}
```

#### **POST /api/v1/instructor-requests/{id}/reject**
Từ chối yêu cầu đăng ký làm giảng viên

**Authorization:** Required (KIEMDUYETVIEN hoặc ADMIN)

**Request:**
```json
{
  "lyDoTuChoi": "string"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Đã từ chối yêu cầu"
}
```

### 6.3. Course APIs

#### **POST /api/v1/courses**
Tạo khóa học mới

**Authorization:** Required (GIANGVIEN)

**Request:**
```json
{
  "tenKhoaHoc": "string",
  "moTaNgan": "string",
  "moTaChiTiet": "string",
  "idDanhMuc": 1,
  "giaBan": 100000,
  "mucDo": "Cơ bản",
  "yeuCauTruoc": "string",
  "hocDuoc": "string",
  "chuongs": [
    {
      "tenChuong": "string",
      "moTa": "string",
      "thuTu": 1,
      "baiGiangs": [
        {
          "tieuDe": "string",
          "moTa": "string",
          "thoiLuong": 3600,
          "thuTu": 1,
          "mienPhiXem": false
        }
      ]
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Tạo khóa học thành công",
  "data": {
    "id": 1,
    "tenKhoaHoc": "string"
  }
}
```

#### **PUT /api/v1/courses/{id}**
Chỉnh sửa khóa học

**Authorization:** Required (GIANGVIEN - owner)

**Request:** Tương tự POST /api/v1/courses

**Response:**
```json
{
  "success": true,
  "message": "Cập nhật khóa học thành công",
  "data": {
    "id": 1,
    "tenKhoaHoc": "string",
    "coHocVienDangKy": true,
    "soLuongHocVien": 5
  }
}
```

**Lưu ý:** Nếu khóa học có học viên đăng ký, hệ thống tự động tạo thông báo cho tất cả học viên.

#### **POST /api/v1/courses/{id}/upload-video**
Upload video bài giảng

**Authorization:** Required (GIANGVIEN - owner)

**Request:** FormData
- video: File
- idChuong: int
- idBaiGiang: int (optional, nếu tạo mới)

**Response:**
```json
{
  "success": true,
  "data": {
    "videoUrl": "string",
    "thoiLuong": 3600
  }
}
```

### 6.4. Enrollment APIs

#### **POST /api/v1/enrollments**
Đăng ký khóa học (sau khi thanh toán thành công)

**Authorization:** Required (HOCVIEN)

**Request:**
```json
{
  "idKhoaHoc": 1,
  "idDonHang": 1
}
```

**Response:**
```json
{
  "success": true,
  "message": "Đăng ký khóa học thành công",
  "data": {
    "idDangKy": 1,
    "idKhoaHoc": 1
  }
}
```

### 6.5. Learning APIs

#### **GET /api/v1/learning/my-courses**
Lấy danh sách khóa học đã đăng ký

**Authorization:** Required (HOCVIEN)

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "tenKhoaHoc": "string",
      "phanTramHoanThanh": 50,
      "soBaiHocDaHoanThanh": 5,
      "tongSoBaiHoc": 10
    }
  ]
}
```

#### **GET /api/v1/learning/courses/{id}/content**
Lấy nội dung khóa học để học

**Authorization:** Required (HOCVIEN - đã đăng ký)

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "tenKhoaHoc": "string",
    "chuongs": [
      {
        "id": 1,
        "tenChuong": "string",
        "baiGiangs": [
          {
            "id": 1,
            "tieuDe": "string",
            "daHoanThanh": false,
            "daMoKhoa": true, // true nếu là bài đầu tiên hoặc bài trước đã hoàn thành
            "videoUrl": "string",
            "taiLieus": []
          }
        ]
      }
    ]
  }
}
```

#### **POST /api/v1/learning/lessons/{id}/complete**
Hoàn thành bài giảng

**Authorization:** Required (HOCVIEN - đã đăng ký khóa học)

**Request:**
```json
{
  "phanTramDaXem": 85 // Phần trăm đã xem video
}
```

**Response:**
```json
{
  "success": true,
  "message": "Hoàn thành bài giảng thành công",
  "data": {
    "baiGiangTiepTheo": {
      "id": 2,
      "tieuDe": "string",
      "daMoKhoa": true
    }
  }
}
```

**Validation:**
- Phải đạt ít nhất 80% thời lượng video mới được hoàn thành
- Bài giảng trước phải đã hoàn thành (trừ bài đầu tiên)

### 6.6. Notification APIs

#### **GET /api/v1/notifications**
Lấy danh sách thông báo

**Authorization:** Required

**Query Parameters:**
- trangThai: string (optional) - Chưa đọc/Đã đọc
- pageNumber: int (default: 1)
- pageSize: int (default: 10)

**Response:**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 1,
        "tieuDe": "string",
        "noiDung": "string",
        "loai": "Khóa học cập nhật",
        "trangThai": "Chưa đọc",
        "ngayTao": "2024-01-01T00:00:00"
      }
    ],
    "totalCount": 10,
    "unreadCount": 5
  }
}
```

#### **PUT /api/v1/notifications/{id}/read**
Đánh dấu thông báo đã đọc

**Authorization:** Required

**Response:**
```json
{
  "success": true,
  "message": "Đã đánh dấu đọc"
}
```

### 6.7. Background Job APIs

#### **POST /api/v1/admin/courses/process-expired-enrollments**
Xử lý đăng ký khóa học hết thời hạn (chạy định kỳ)

**Authorization:** Required (ADMIN hoặc System)

**Response:**
```json
{
  "success": true,
  "message": "Đã xử lý đăng ký hết thời hạn",
  "data": {
    "soDangKyHetHan": 5,
    "soThongBaoGui": 5
  }
}
```

---

## 7. BUSINESS RULES

### 7.1. Quy tắc Đăng ký làm Giảng viên

1. Chỉ HOCVIEN mới có thể gửi yêu cầu đăng ký làm giảng viên
2. HOCVIEN đã có role GIANGVIEN không thể gửi yêu cầu lại
3. Chứng chỉ phải là file ảnh (JPG, PNG) hoặc PDF
4. Kích thước file tối đa: 10MB
5. Yêu cầu phải được duyệt bởi KIEMDUYETVIEN hoặc ADMIN
6. Khi duyệt, học viên được thêm role GIANGVIEN (không xóa role HOCVIEN)

### 7.2. Quy tắc Khóa học

1. Chỉ GIANGVIEN mới có thể tạo khóa học
2. Khóa học phải có ít nhất 1 chương
3. Mỗi chương phải có ít nhất 1 bài giảng
4. Mỗi bài giảng phải có video
5. Khóa học cần được ADMIN duyệt trước khi publish
6. Khóa học không có thời hạn tự động hết hạn, tồn tại vĩnh viễn trong hệ thống
7. Học viên có thời hạn truy cập sau khi thanh toán (ví dụ: 6 tháng hoặc 12 tháng)
8. Sau khi hết thời hạn, học viên không thể truy cập và phải gia hạn bằng cách thanh toán lại

### 7.3. Quy tắc Chỉnh sửa Khóa học

1. Giảng viên có thể chỉnh sửa khóa học bất cứ lúc nào
2. Khi chỉnh sửa khóa học có học viên đăng ký:
   - Phải gửi thông báo đến tất cả học viên đã đăng ký
   - Thông báo: "Khóa học '[Tên khóa học]' đã được cập nhật"
3. Không thể chỉnh sửa giá sau khi đã có học viên đăng ký

### 7.4. Quy tắc Học tập

1. Học viên chỉ có thể học khóa học đã đăng ký
2. Phải học tuần tự: hoàn thành bài giảng trước mới được học bài giảng tiếp theo
3. Hoàn thành bài giảng = xem hết video (hoặc đạt 80% thời lượng)
4. Bài giảng đầu tiên luôn được mở khóa
5. Học viên có thể xem lại bài giảng đã hoàn thành

### 7.5. Quy tắc Thanh toán

1. Chỉ HOCVIEN mới có thể đăng ký khóa học
2. Không thể đăng ký lại khóa học đã đăng ký và chưa hết thời hạn
3. Có thể gia hạn khóa học đã hết thời hạn bằng cách thanh toán lại
4. Thanh toán một lần cho mỗi lần đăng ký/gia hạn
5. Doanh thu được chia: 70% giảng viên, 30% admin
6. Thời hạn truy cập được tính từ ngày thanh toán thành công

### 7.6. Quy tắc Kiểm duyệt viên

1. KIEMDUYETVIEN chỉ có quyền kiểm duyệt yêu cầu đăng ký làm giảng viên
2. KIEMDUYETVIEN không có quyền của HOCVIEN:
   - Không thể xem danh sách khóa học
   - Không thể đăng ký khóa học
   - Không thể học tập
3. KIEMDUYETVIEN chỉ có thể truy cập:
   - Trang đăng nhập
   - Trang kiểm duyệt

---

## 8. VALIDATION RULES

### 8.1. Validation Đăng ký Tài khoản

- Họ tên: Bắt buộc, tối đa 100 ký tự
- Email: Bắt buộc, định dạng email hợp lệ, không trùng
- Mật khẩu: Bắt buộc, tối thiểu 6 ký tự
- Xác nhận mật khẩu: Bắt buộc, phải khớp với mật khẩu

### 8.2. Validation Đăng ký làm Giảng viên

- Chứng chỉ: Bắt buộc, file ảnh (JPG, PNG) hoặc PDF, tối đa 10MB
- Thông tin bổ sung: Optional, tối đa 5000 ký tự

### 8.3. Validation Khóa học

- Tên khóa học: Bắt buộc, tối đa 255 ký tự
- Mô tả ngắn: Bắt buộc, tối đa 500 ký tự
- Mô tả chi tiết: Optional
- Danh mục: Bắt buộc, phải tồn tại
- Giá bán: Bắt buộc, >= 0
- Hình đại diện: Bắt buộc, file ảnh
- Mức độ: Bắt buộc, một trong: Cơ bản/Trung bình/Nâng cao
- Số chương: >= 1
- Số bài giảng mỗi chương: >= 1
- Video bài giảng: Bắt buộc cho mỗi bài giảng

### 8.4. Validation Đăng ký Khóa học

- Khóa học phải tồn tại
- Khóa học phải đã được publish
- Học viên chưa đăng ký khóa học này hoặc đã hết thời hạn truy cập

### 8.5. Validation Hoàn thành Bài giảng

- Học viên phải đã đăng ký khóa học
- Bài giảng phải thuộc khóa học đã đăng ký
- Bài giảng phải được mở khóa (là bài đầu tiên hoặc bài trước đã hoàn thành)
- Phần trăm đã xem video >= 80%

---

## 9. ERROR HANDLING

### 9.1. Error Codes

- **400 Bad Request:** Dữ liệu không hợp lệ
- **401 Unauthorized:** Chưa đăng nhập
- **403 Forbidden:** Không có quyền truy cập
- **404 Not Found:** Không tìm thấy tài nguyên
- **500 Internal Server Error:** Lỗi server

### 9.2. Error Response Format

```json
{
  "success": false,
  "message": "Mô tả lỗi",
  "errors": [
    {
      "field": "email",
      "message": "Email không hợp lệ"
    }
  ]
}
```

### 9.3. Common Errors

- **Email đã tồn tại:** Khi đăng ký với email đã có
- **Email hoặc mật khẩu sai:** Khi đăng nhập
- **Không có quyền:** Khi truy cập tài nguyên không thuộc về mình
- **Thời hạn truy cập đã hết hạn:** Khi học viên cố gắng truy cập khóa học đã hết thời hạn
- **Chưa hoàn thành bài giảng trước:** Khi học bài giảng tiếp theo

---

## 10. UI/UX REQUIREMENTS

### 10.1. Trang Chủ

- Hiển thị danh sách khóa học công khai
- Tìm kiếm và lọc khóa học
- Danh sách đặc biệt (nổi bật, bán chạy, mới nhất)

### 10.2. Trang Chi tiết Khóa học

- Thông tin cơ bản
- Hình ảnh khóa học
- Danh sách chương và bài giảng
- Đánh giá từ học viên
- Nút "Thêm vào giỏ hàng" hoặc "Đã đăng ký" hoặc "Gia hạn" (nếu đã hết hạn)

### 10.3. Dashboard Giảng viên

- Tổng số khóa học
- Tổng số học viên
- Tổng doanh thu
- Đánh giá trung bình
- Danh sách khóa học
- Nút "Tạo khóa học mới"

### 10.4. Trang Kiểm duyệt

- Danh sách yêu cầu đăng ký làm giảng viên
- Lọc theo trạng thái
- Xem chi tiết và duyệt/từ chối

### 10.5. Trang Học tập

- Danh sách khóa học đã đăng ký
- Player video
- Danh sách bài giảng với trạng thái (đã hoàn thành/chưa mở khóa)
- Tiến độ học tập

### 10.6. Thông báo

- Icon thông báo trên header
- Số lượng thông báo chưa đọc
- Danh sách thông báo
- Đánh dấu đã đọc

---

## 11. TECHNICAL REQUIREMENTS

### 11.1. Performance

- API response time < 500ms cho các request thông thường
- Upload video: Hỗ trợ file lớn, có progress bar
- Background job chạy mỗi ngày để xử lý đăng ký khóa học hết thời hạn truy cập

### 11.2. Security

- JWT token với expiration time
- Refresh token để renew access token
- Hash mật khẩu bằng BCrypt
- Validate file upload (type, size)
- CORS configuration
- SQL injection prevention

### 11.3. Storage

- Video và ảnh lưu trên cloud storage hoặc local storage
- File chứng chỉ lưu trên storage
- Backup database định kỳ

---

## 12. TESTING REQUIREMENTS

### 12.1. Unit Tests

- Test các service methods
- Test validation rules
- Test business logic

### 12.2. Integration Tests

- Test API endpoints
- Test database operations
- Test payment integration

### 12.3. E2E Tests

- Test quy trình đăng ký và học tập
- Test quy trình tạo và quản lý khóa học
- Test quy trình kiểm duyệt

---

## KẾT LUẬN

Tài liệu này mô tả đầy đủ các yêu cầu và đặc tả kỹ thuật của hệ thống quản lý khóa học trực tuyến. Tất cả các tính năng, quy trình, API, và business rules đã được mô tả chi tiết để có thể triển khai chính xác.


