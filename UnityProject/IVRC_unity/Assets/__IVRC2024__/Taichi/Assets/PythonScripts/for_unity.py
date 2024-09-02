import os
import sys
import cv2  # ここで初めてcv2をインポート
import mediapipe as mp  # ここで初めてmediapipeをインポート
import time

# MediaPipeのセットアップ
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(max_num_hands=1, min_detection_confidence=0.7)

# カメラキャプチャの開始
cap = cv2.VideoCapture(0)

if cap.isOpened():
    print("UNITY: please wave your hand...")  # カメラが正常に開かれたらメッセージを表示
    sys.stdout.flush()  # 出力を即座にフラッシュ

prev_x = None
wave_count = 0


try:
    while True:
        ret, frame = cap.read()
        if not ret:
            print("Dont get frame")
            break

        # BGRからRGBに変換
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        result = hands.process(rgb_frame)

        if result.multi_hand_landmarks:
            for hand_landmarks in result.multi_hand_landmarks:
                wrist = hand_landmarks.landmark[mp_hands.HandLandmark.WRIST]
                current_x = wrist.x

                if prev_x is not None:
                    x_diff = abs(current_x - prev_x)
                    print(f"Displacement of hand movement: {x_diff:.4f}")

                    if x_diff > 0.05:
                        wave_count += 1
                        last_wave_time = time.time()
                        print(f"Number of times waved: {wave_count}")

                prev_x = current_x

        if wave_count > 1:
            print("UNITY: Waving motion detected!")
            sys.stdout.flush()  # 出力を即座にUnityに送る
            break

        time.sleep(0.1)

finally:
    cap.release()
    hands.close()
    print("Exit Program...")
