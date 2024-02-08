using System;
using System.IO.Ports;

namespace util
{
     class ModBusRTU: IDisposable
    {
        private static readonly object _lock = new object();
        private SerialPort _serialPort;
        private static ModBusRTU _instance;

        private ModBusRTU(string portName, int baudRate)
        {
            _serialPort = new SerialPort(portName, baudRate);
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;
            // 읽기 타임아웃 설정 (1초)
            _serialPort.ReadTimeout = 1000; 
            // 쓰기 타임아웃 설정 (1초)
            _serialPort.WriteTimeout = 1000; 
            // ErrorReceived 이벤트 핸들러 등록
            _serialPort.ErrorReceived += Error_Handler;

            try
            {
                // 시리얼 포트 열기
                _serialPort.Open(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("::ERR: " + ex.Message);
            }
        }

        public static ModBusRTU GetInstance(string portName, int baudRate) 
        {
            lock (_lock) 
            {
                if (_instance == null) 
                {
                    _instance = new ModBusRTU(portName, baudRate);
                }
                return _instance;
            }
        }

        public void SendMessage(byte slaveAddress, byte functionCode, ushort startAddress, ushort data)
        {
            lock (_lock)
            {
                try
                {
                    // Modbus RTU 프레임 구성
                    byte[] frame = new byte[] {
                                                slaveAddress,
                                                functionCode,
                                                (byte)(startAddress >> 8), // 시작 주소 상위 바이트
                                                (byte)(startAddress & 0xFF), // 시작 주소 하위 바이트
                                                (byte)(data >> 8), // 데이터 상위 바이트
                                                (byte)(data & 0xFF), // 데이터 하위 바이트
                                                // CRC (Cyclic Redundancy Check) 계산하여 추가
                                                // CRC 계산 로직은 생략
                                            };

                    _serialPort.Write(frame, 0, frame.Length); // 시리얼 포트로 프레임 전송
                }
                catch (Exception ex)
                {
                    Console.WriteLine(": " + ex.Message);
                }
            }
        }

        private void Error_Handler(object sender, SerialErrorReceivedEventArgs e)
        {
            if (e.EventType == SerialError.RXOver || e.EventType == SerialError.Overrun || e.EventType == SerialError.RXParity || e.EventType == SerialError.Frame)
            {
                // todo: 시리얼 통신 오류 처리 (연결 끊김 등)
                Console.WriteLine("Serial port error occurred: " + e.EventType.ToString());
                // todo: 여기에 연결 끊김에 대한 추가 처리 로직 작성
            }
        }

        public void ReceiveMessage()
        {
            lock (_lock)
            {
                try
                {
                    // 응답 메시지 수신
                    byte[] buffer = new byte[256];
                    int bytesRead = _serialPort.Read(buffer, 0, buffer.Length);

                    // 받은 데이터 처리
                    if (bytesRead > 0)
                    {
                        Console.WriteLine(":: Received message: " + BitConverter.ToString(buffer, 0, bytesRead));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("::ERR: " + ex.Message);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    if (_serialPort != null)
                    {
                        _serialPort.Close(); // 시리얼 포트 닫기
                        _serialPort.Dispose();
                        _serialPort = null;
                    }
                }
            }
        }

    }
}
