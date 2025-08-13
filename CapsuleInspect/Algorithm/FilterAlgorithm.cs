using CapsuleInspect.Core;
using CapsuleInspect.Property;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
namespace CapsuleInspect.Algorithm
{
    public class FilterAlgorithm : InspAlgorithm
    {
        public FilterType Filter { get; set; } = FilterType.None;
        [XmlIgnore]
        public dynamic Options { get; set; } = null;
        [XmlIgnore]
        public new Mat ResultImage { get; private set; } // new 키워드 추가
        public FilterAlgorithm()
        {
            InspectType = InspectType.InspFilter;
        }
        // InspWindow 복사를 위한 FilterAlgorithm 복사 함수
        public override InspAlgorithm Clone()
        {
            var cloneAlgo = new FilterAlgorithm();

            // 공통 필드 복사
            this.CopyBaseTo(cloneAlgo);

            cloneAlgo.CopyFrom(this);

            return cloneAlgo;
        }

        public override bool CopyFrom(InspAlgorithm sourceAlgo)
        {
            FilterAlgorithm filterAlgo = (FilterAlgorithm)sourceAlgo;

            this.Filter = filterAlgo.Filter;

            // Options는 dynamic이므로, null 체크 후 깊은 복사 시도
            if (filterAlgo.Options != null)
            {
                // Options의 실제 타입에 따라 적절히 복사
                // 예: FlipMode, Resize 옵션 등은 단순 값이므로 새 객체로 복사
                this.Options = new
                {
                    FlipMode = filterAlgo.Options?.FlipMode,
                    Direction = filterAlgo.Options?.Direction,
                    Fx = filterAlgo.Options?.Fx,
                    Fy = filterAlgo.Options?.Fy,
                    Min = filterAlgo.Options?.Min,
                    Max = filterAlgo.Options?.Max,
                    MorphType = filterAlgo.Options?.MorphType,
                    Angle = filterAlgo.Options?.Angle
                };
            }
            else
            {
                this.Options = null;
            }

            return true;
        }
        public new void SetSourceImage(Mat srcImage)
        {
            _srcImage = srcImage?.Clone();
        }

        public static Mat Apply(Mat src, FilterType filter, dynamic options = null)
        {
            Mat dst = new Mat();

            switch (filter)
            {
                case FilterType.Grayscale:
                    if (src.Channels() == 1)
                    {
                        dst = src.Clone(); // 이미 흑백이면 그대로 복사
                    }
                    else
                    {
                        Cv2.CvtColor(src, dst, ColorConversionCodes.BGR2GRAY);
                    }
                    break;

                case FilterType.HSVscale:
                    if (src.Channels() == 1)
                    {
                        MessageBox.Show("흑백 이미지에는 HSV 변환을 적용할 수 없습니다.");
                        dst = src.Clone();
                    }
                    else
                    {
                        Cv2.CvtColor(src, dst, ColorConversionCodes.BGR2HSV);
                    }
                    break;
                case FilterType.Flip:
                    FlipMode mode = (FlipMode)options.FlipMode;
                    Cv2.Flip(src, dst, mode);
                    break;
                case FilterType.Pyramid:
                    string direction = options?.Direction ?? "Down";
                    if (direction == "Up")
                        Cv2.PyrUp(src, dst);
                    else
                        Cv2.PyrDown(src, dst);
                    break;
                case FilterType.Resize:
                    double fx = options.Fx / 100.0;
                    double fy = options.Fy / 100.0;
                    Cv2.Resize(src, dst, new Size(), fx, fy);
                    break;
                case FilterType.CannyEdge:
                    {
                        Mat gray = new Mat();

                        // 이미지가 컬러일 경우에만 흑백으로 변환
                        if (src.Channels() == 3 || src.Channels() == 4)
                        {
                            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                        }
                        else
                        {
                            gray = src.Clone(); // 이미 흑백이면 그대로 복사
                        }

                        // 이진화
                        Cv2.Canny(gray, dst, options.Min, options.Max);
                    }
                    break;
                case FilterType.Morphology:
                    {
                        Mat binary = new Mat();
                        if (src.Channels() == 1)
                            binary = src.Clone();
                        else
                            Cv2.CvtColor(src, binary, ColorConversionCodes.BGR2GRAY);
                        Cv2.Threshold(binary, binary, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
                        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));

                        MorphTypes morphType = options.MorphType;
                        Cv2.MorphologyEx(binary, dst, morphType, kernel);
                    }
                    break;
                case FilterType.Rotation:
                    {

                        double angle = (double)options.Angle;

                        Point2f center = new Point2f(src.Width / 2f, src.Height / 2f);
                        Mat matrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);
                        Cv2.WarpAffine(src, dst, matrix, new Size(src.Width, src.Height),
                                       InterpolationFlags.Linear, BorderTypes.Constant, Scalar.All(0));
                    }
                    break;
                default:
                    dst = src.Clone();
                    break;
            }

            return dst;
        }
        // InspAlgorithm 상속으로 인해 필수 구현: BlobAlgorithm과 유사한 구조
        public override bool DoInspect()
        {
            ResetResult();

            if (_srcImage == null)
            {
                ResultString.Add("Error: Source image is null");
                return false;
            }

            // Apply를 호출하여 필터 적용 (프로퍼티로 저장된 Filter와 Options 사용)
            Mat filteredImage = Apply(_srcImage, Filter, Options);
            // 필터링 결과를 InspStage에 저장
            Global.Inst.InspStage.SetFilteredImage(filteredImage);
            // 필터 적용 결과를 _srcImage에 업데이트 (후속 검사나 표시를 위해)
            // 필요 시 이 부분을 조정 (예: 별도 결과 저장)
            ResultImage = filteredImage.Clone();
            _srcImage = filteredImage.Clone();

            // 검사 완료 표시 (BlobAlgorithm과 유사)
            IsInspected = true;

            // 필터는 일반적으로 '불량' 개념이 아니므로 IsDefect는 false로 유지
            // 필요 시 ResultString에 필터 적용 정보 추가 (예: "Filter applied: " + Filter.ToString())
            ResultString.Add($"Applied filter: {Filter}");

            return true;
        }

        // 검사 결과 초기화 (BlobAlgorithm과 유사)
        public override void ResetResult()
        {
            base.ResetResult();
            ResultImage?.Dispose();
            ResultImage = null;
        }
    }
}
