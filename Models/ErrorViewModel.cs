namespace MVCTest1.Models
{
    // ���� ������ ��� �� Ŭ����
    public class ErrorViewModel
    {
        // ��û ID. ������ �߻��� ��û�� �����ϱ� ���� ���˴ϴ�.
        public string? RequestId { get; set; }

        // RequestId�� ������� ���� ��� true�� ��ȯ�մϴ�. �信�� RequestId�� ǥ�� ���θ� �����ϱ� ���� ���˴ϴ�.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
