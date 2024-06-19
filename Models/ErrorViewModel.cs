namespace MVCTest1.Models
{
    // 에러 정보를 담는 모델 클래스
    public class ErrorViewModel
    {
        // 요청 ID. 에러가 발생한 요청을 구분하기 위해 사용됩니다.
        public string? RequestId { get; set; }

        // RequestId가 비어있지 않을 경우 true를 반환합니다. 뷰에서 RequestId의 표시 여부를 결정하기 위해 사용됩니다.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
