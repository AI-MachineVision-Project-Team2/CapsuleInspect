using CapsuleInspect.Util;
using MvCamCtrl.NET;
using MvCameraControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapsuleInspect.Grab
{

    //GrabModel로 상속 변경
    //internal class HikRobotCam : IDisposable
    internal class HikRobotCam : GrabModel
    {
        private IDevice _device = null;

        // 이미지 취득 콜백함수
        void FrameGrabedEventHandler(object sender, FrameGrabbedEventArgs e)
        {
            
            IFrameOut frameOut = e.FrameOut;

            // 영상 취득이 완료되었을 때 이벤트 발생
            OnGrabCompleted(BufferIndex);

            if (_userImageBuffer[BufferIndex].ImageBuffer != null)
            {
                if (frameOut.Image.PixelType == MvGvspPixelType.PixelType_Gvsp_Mono8)
                {
                    if (_userImageBuffer[BufferIndex].ImageBuffer != null)
                    {
                        IntPtr ptrSourceTemp = frameOut.Image.PixelDataPtr;
                        Marshal.Copy(ptrSourceTemp, _userImageBuffer[BufferIndex].ImageBuffer, 0, (int)frameOut.Image.ImageSize);
                    }
                }
                else
                {
                    IImage inputImage = frameOut.Image;
                    IImage outImage;
                    MvGvspPixelType dstPixelType = MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;

                    // Pixel type convert 
                    int result = _device.PixelTypeConverter.ConvertPixelType(inputImage, out outImage, dstPixelType);
                    if (result != MvError.MV_OK)
                    {
                        SLogger.Write($"!경고! 이미지 변환 실패:{result:x8}", SLogger.LogType.Error);
                        return;
                    }

                    if (_userImageBuffer[BufferIndex].ImageBuffer != null)
                    {
                        IntPtr ptrSourceTemp = outImage.PixelDataPtr;
                        Marshal.Copy(ptrSourceTemp, _userImageBuffer[BufferIndex].ImageBuffer, 0, (int)outImage.ImageSize);
                    }
                }
            }

            // 영상 전송이 완료되었을 때 이벤트 발생
            OnTransferCompleted(BufferIndex);

            //IO 트리거 촬상시 최대 버퍼를 넘으면 첫번째 버퍼로 변경
            if (IncreaseBufferIndex)
            {
                BufferIndex++;
                if (BufferIndex >= _userImageBuffer.Count())
                    BufferIndex = 0;
            }
        }

        #region Method
        //GrabModel에서 상속받은 함수를 위해 override 추가
        internal override bool Create(string strIpAddr = null)
        {
            // Initialize SDK
            SDKSystem.Initialize();

            _strIpAddr = strIpAddr;

            try
            {
                const DeviceTLayerType devLayerType = DeviceTLayerType.MvGigEDevice;

                List<IDeviceInfo> devInfoList;

                // Enum device
                int ret = DeviceEnumerator.EnumDevices(devLayerType, out devInfoList);
                if (ret != MvError.MV_OK)
                {
                    SLogger.Write($"!경고! 장치 검색 실패:{ret:x8}", SLogger.LogType.Error);
                    return false;
                }

                SLogger.Write($"검색된 장치 수 : {devInfoList.Count}");

                if (0 == devInfoList.Count)
                {
                    return false;
                }

                int selDevIndex = -1;

                // Print device info
                int devIndex = 0;
                foreach (var devInfo in devInfoList)
                {
                    
                    if (devInfo.TLayerType == DeviceTLayerType.MvGigEDevice || devInfo.TLayerType == DeviceTLayerType.MvVirGigEDevice || devInfo.TLayerType == DeviceTLayerType.MvGenTLGigEDevice)
                    {
                        IGigEDeviceInfo gigeDevInfo = devInfo as IGigEDeviceInfo;
                        uint nIp1 = ((gigeDevInfo.CurrentIp & 0xff000000) >> 24);
                        uint nIp2 = ((gigeDevInfo.CurrentIp & 0x00ff0000) >> 16);
                        uint nIp3 = ((gigeDevInfo.CurrentIp & 0x0000ff00) >> 8);
                        uint nIp4 = (gigeDevInfo.CurrentIp & 0x000000ff);

                        string strIP = nIp1 + "." + nIp2 + "." + nIp3 + "." + nIp4;
                        SLogger.Write($"장치 : {devIndex}, IP주소 : " + strIP);

                        if (_strIpAddr is null || strIP == strIpAddr)
                        {
                            selDevIndex = devIndex;
                            break;
                        }
                    }

                    SLogger.Write("모델이름 :" + devInfo.ModelName);
                    SLogger.Write("시리얼 넘버:" + devInfo.SerialNumber);

                    devIndex++;
                }

                if (selDevIndex < 0 || selDevIndex > devInfoList.Count - 1)
                {
                    SLogger.Write($"!경고! 선택한 장치 번호가 잘못되었습니다:{selDevIndex}", SLogger.LogType.Error);
                    return false;
                }

                // Create device
                _device = DeviceFactory.CreateDevice(devInfoList[selDevIndex]);

                _disposed = false;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                ex.ToString();
                return false;
            }
            return true;
        }
        internal override bool Grab(int bufferIndex, bool waitDone)
        {
            if (_device == null)
                return false;

            BufferIndex = bufferIndex;
            bool ret = true;

            if (!HardwareTrigger)
            {
                try
                {
                    int result = _device.Parameters.SetCommandValue("TriggerSoftware");
                    if (result != MvError.MV_OK)
                    {
                        ret = false;
                    }
                }
                catch
                {
                    ret = false;
                }
            }

            return ret;
        }

        internal override bool Close()
        {
            if (_device != null)
            {
                _device.StreamGrabber.StopGrabbing();
                _device.Close();
            }

            return true;
        }

        internal override bool Open()
        {
            try
            {
                if (_device == null)
                    return false;

                if (!_device.IsConnected)
                {
                    int ret = _device.Open();
                    if (MvError.MV_OK != ret)
                    {
                        _device.Dispose();
                        SLogger.Write($"!경고! HikRobot 장치 열기 실패! [{ret:x8}]", SLogger.LogType.Error);
                        MessageBox.Show($"HikRobot 장치 열기 실패! {ret:X8}");
                        return false;
                    }

                    if (_device is IGigEDevice)
                    {
                        int packetSize;
                        ret = (_device as IGigEDevice).GetOptimalPacketSize(out packetSize);
                        if (packetSize > 0)
                        {
                            ret = _device.Parameters.SetIntValue("GevSCPSPacketSize", packetSize);
                            if (ret != MvError.MV_OK)
                            {
                                SLogger.Write($"!경고! 패킷 크기 설정 실패:{ret:x8}", SLogger.LogType.Error);
                            }
                            else
                            {
                                SLogger.Write($"패킷 크기를 {packetSize}으로 설정했습니다");
                            }
                        }
                        else
                        {
                            SLogger.Write($"!경고! 패킷 크기 설정 실패:{ret:x8}", SLogger.LogType.Error);
                        }
                    }

                    // set trigger mode as off
                    ret = _device.Parameters.SetEnumValue("TriggerMode", 1);
                    if (ret != MvError.MV_OK)
                    {
                        SLogger.Write($"!경고! 트리거 모드 설정 실패:{ret:x8}", SLogger.LogType.Error);
                        return false;
                    }

                    if (HardwareTrigger)
                    {
                        _device.Parameters.SetEnumValueByString("TriggerSource", "Line0");
                    }
                    else
                    {
                        _device.Parameters.SetEnumValueByString("TriggerSource", "Software");
                    }

                    // Register image callback
                    _device.StreamGrabber.FrameGrabedEvent += FrameGrabedEventHandler;

                    // start grab image
                    ret = _device.StreamGrabber.StartGrabbing();
                    if (ret != MvError.MV_OK)
                    {
                        SLogger.Write("$ !경고! 그랩 시작 실패:{ret:x8}", SLogger.LogType.Error);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                SLogger.Write(ex.ToString(), SLogger.LogType.Error);
                return false;
            }

            return true;
        }

        internal override bool Reconnect()
        {
            if (_device is null)
            {
                SLogger.Write("_device is null", SLogger.LogType.Error);
                return false;
            }
            Close();
            return Open();
        }

        internal override bool GetPixelBpp(out int pixelBpp)
        {
            pixelBpp = 8;
            if (_device == null)
                return false;

            IEnumValue enumValue;
            int result = _device.Parameters.GetEnumValue("PixelFormat", out enumValue);
            if (result != MvError.MV_OK)
            {
                SLogger.Write($"!경고! 픽셀 포맷 조회 실패:{result:x8}", SLogger.LogType.Error);
                return false;
            }

            if (MvGvspPixelType.PixelType_Gvsp_Mono8 == (MvGvspPixelType)enumValue.CurEnumEntry.Value)
                pixelBpp = 8;
            else
                pixelBpp = 24;

            return true;
        }
        #endregion


        #region Parameter Setting
        internal override bool SetExposureTime(long exposure)
        {
            if (_device == null)
                return false;

            _device.Parameters.SetEnumValue("ExposureAuto", 0);
            int result = _device.Parameters.SetFloatValue("ExposureTime", exposure);
            if (result != MvError.MV_OK)
            {
                SLogger.Write($"!경고! 노출 시간 설정 실패:{result:x8}", SLogger.LogType.Error);
                return false;
            }

            return true;
        }

        internal override bool GetExposureTime(out long exposure)
        {
            exposure = 0;
            if (_device == null)
                return false;

            IFloatValue floatValue;
            int result = _device.Parameters.GetFloatValue("ExposureTime", out floatValue);
            if (result == MvError.MV_OK)
            {
                exposure = (long)floatValue.CurValue;
            }

            return true;
        }

        internal override bool SetGain(float gain)
        {
            if (_device == null)
                return false;

            _device.Parameters.SetEnumValue("GainAuto", 0);
            int result = _device.Parameters.SetFloatValue("Gain", gain);
            if (result != MvError.MV_OK)
            {
                SLogger.Write($"!경고! 이득(Gain) 값 설정 실패:{result:x8}", SLogger.LogType.Error);

                return false;
            }

            return true;
        }

        internal override bool GetGain(out float gain)
        {
            gain = 0;
            if (_device == null)
                return false;

            IFloatValue floatValue;
            int result = _device.Parameters.GetFloatValue("Gain", out floatValue);
            if (result == MvError.MV_OK)
            {
                gain = floatValue.CurValue;
            }

            return true;
        }

        internal override bool GetResolution(out int width, out int height, out int stride)
        {
            width = 0;
            height = 0;
            stride = 0;

            if (_device == null)
                return false;

            IIntValue intValue;
            IEnumValue enumValue;
            MvGvspPixelType pixelType;

            int result;

            result = _device.Parameters.GetIntValue("Width", out intValue);
            if (result != MvError.MV_OK)
            {
                SLogger.Write($"!경고! 이득(Gain) 값 설정 실패:{result:x8}", SLogger.LogType.Error);
                return false;
            }
            width = (int)intValue.CurValue;

            result = _device.Parameters.GetIntValue("Height", out intValue);
            if (result != MvError.MV_OK)
            {
                SLogger.Write($"!경고! 이미지 높이 값 읽기 실패:{result:x8}", SLogger.LogType.Error);
                return false;
            }
            height = (int)intValue.CurValue;

            result = _device.Parameters.GetEnumValue("PixelFormat", out enumValue);
            if (result != MvError.MV_OK)
            {
                SLogger.Write($"!경고! 픽셀 형식 읽기 실패:{result:x8}", SLogger.LogType.Error);
                return false;
            }
            pixelType = (MvGvspPixelType)enumValue.CurEnumEntry.Value;

            if (pixelType == MvGvspPixelType.PixelType_Gvsp_Mono8)
                stride = width * 1;
            else
                stride = width * 3;

            return true;
        }

        internal override bool SetTriggerMode(bool hardwareTrigger)
        {
            if (_device is null)
                return false;

            HardwareTrigger = hardwareTrigger;

            if (HardwareTrigger)
            {
                _device.Parameters.SetEnumValueByString("TriggerSource", "Line0");
            }
            else
            {
                _device.Parameters.SetEnumValueByString("TriggerSource", "Software");
            }

            return true;
        }

        #endregion

        #region Dispose

        private bool _disposed = false;

        protected void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_device != null)
                {
                    _device.StreamGrabber.FrameGrabedEvent -= FrameGrabedEventHandler;
                    _device.StreamGrabber.StopGrabbing();
                    _device.Close();
                    _device.Dispose();
                    _device = null;

                    // Finalize SDK
                    SDKSystem.Finalize();
                }
            }
            _disposed = true;
        }

        internal override void Dispose()
        {
            Dispose(disposing: true);
        }
        #endregion //Disposable
    }
}
