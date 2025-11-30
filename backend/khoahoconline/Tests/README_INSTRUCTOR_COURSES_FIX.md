# Hướng dẫn Test và Sửa lỗi API Get Courses by Instructor

## Vấn đề
API endpoint `/api/v1/courses/instructor/{instructorId}` trả về lỗi 500 Internal Server Error khi:
- Xem giao diện tổng quan của giảng viên
- Xem giao diện quản lý khóa học của giảng viên

## Nguyên nhân
1. **Backend**: Thiếu xử lý exception trong `KhoaHocController.GetByInstructor`
2. **Backend**: Lỗi trong `KhoaHocService.GetByInstructorAsync` khi xử lý danh sách khóa học (async loop có thể throw exception)
3. **Frontend**: Không validate `user.id` trước khi gọi API, có thể gửi giá trị undefined/null
4. **Frontend**: Không xử lý lỗi chi tiết từ API response

## Các thay đổi đã thực hiện

### 1. Backend - KhoaHocController.cs
- ✅ Thêm try-catch để xử lý exception
- ✅ Validate input (instructorId, pageNumber, pageSize)
- ✅ Trả về error message chi tiết cho từng loại lỗi

### 2. Backend - KhoaHocService.cs
- ✅ Thêm try-catch trong `GetByInstructorAsync`
- ✅ Thay đổi từ async Select sang foreach loop để xử lý lỗi từng course riêng biệt
- ✅ Nếu một course có lỗi, skip course đó thay vì fail toàn bộ request

### 3. Frontend - instructor-dashboard.js
- ✅ Validate và convert `user.id` sang number
- ✅ Kiểm tra `user.id` hợp lệ trước khi gọi API
- ✅ Hiển thị thông báo lỗi rõ ràng hơn

### 4. Frontend - courseApi.js
- ✅ Validate input trước khi gọi API
- ✅ Xử lý error response chi tiết hơn
- ✅ Hiển thị thông báo lỗi phù hợp với từng status code

## Cách test

### Test 1: Sử dụng HTTP file (REST Client)
1. Mở file `InstructorCoursesApiTest.http` trong VS Code
2. Cài đặt extension "REST Client" nếu chưa có
3. Chạy từng test case bằng cách click vào "Send Request" phía trên mỗi request

### Test 2: Test qua giao diện
1. Đăng nhập với tài khoản giảng viên
2. Truy cập trang instructor dashboard
3. Kiểm tra:
   - Giao diện tổng quan hiển thị đúng
   - Danh sách khóa học hiển thị đúng
   - Không có lỗi 500 trong console

### Test 3: Test với các trường hợp edge case
1. Test với instructorId không tồn tại → Phải trả về 404
2. Test với instructorId = 0 → Phải trả về 400 Bad Request
3. Test với pageNumber < 1 → Phải trả về 400 Bad Request
4. Test với pageSize > 100 → Phải trả về 400 Bad Request

## Kết quả mong đợi
- ✅ API trả về 200 OK với dữ liệu hợp lệ
- ✅ API trả về 404 khi instructor không tồn tại
- ✅ API trả về 400 với error message rõ ràng khi input không hợp lệ
- ✅ Frontend hiển thị thông báo lỗi rõ ràng thay vì crash
- ✅ Console không còn lỗi 500

## Lưu ý
- Đảm bảo backend đang chạy trên `http://localhost:5228`
- Đảm bảo database có dữ liệu test (ít nhất 1 giảng viên và khóa học)
- Kiểm tra log trong backend console để xem chi tiết lỗi nếu có



