import numpy as np
import cv2
import matplotlib.pyplot as plt
import copy
import plotly.graph_objects as go
from cpn import load_model, image_preprocessing
from person_detection import crop_human
from pose_3d import Pose3DCPN
import socket
import time
import warnings
prev= 0
new= 0
warnings.filterwarnings("ignore", category=np.VisibleDeprecationWarning) 
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(("127.0.0.1", 25003))
R1 = np.array([[-0.1434, 0.9896, 0.0092],
              [0.1672, 0.0334, -0.9854],
              [-0.9754, -0.1397, -0.1702]])
R1_inv = np.linalg.inv(R1)
T1 = np.array([[-88.1, 804.8, 4315.3]]).T

R2 = np.array([[0.9654, 0.2600, -0.0214],
              [0.0151, -0.1372, -0.9904],
              [-0.2605, 0.9558, -0.1364]])
R2_inv = np.linalg.inv(R2)
T2 = np.array([[206.1, 955.7, 4062.3]]).T
pose_3d_model = Pose3DCPN(R1, T1, R2, T2)
cap_1 = cv2.VideoCapture(r"C:\detectiongpu\1.avi")
cap_2 = cv2.VideoCapture(r"C:\detectiongpu\2.avi")
while cap_1.isOpened() or cap_2.isOpened():
    _,cam_1=cap_1.read()
    _,cam_2=cap_2.read()
    crop_img_1, bbox_1 = crop_human(cam_1[:,:,[2,1,0]])
    crop_img_2, bbox_2 = crop_human(cam_2[:,:,[2,1,0]])
    crop_img_1 = cv2.cvtColor(crop_img_1, cv2.COLOR_BGR2RGB)
    crop_img_2 = cv2.cvtColor(crop_img_2, cv2.COLOR_BGR2RGB)
    img_1 = cv2.resize(crop_img_1, (288, 384))
    img_2 = cv2.resize(crop_img_2, (288, 384))
    pimg_1 = image_preprocessing(img_1)
    pimg_2 = image_preprocessing(img_2)
    heatmap_1 = pose_3d_model.gen_heatmaps(img_1, pimg_1)
    heatmap_2 = pose_3d_model.gen_heatmaps(img_2, pimg_2)
    kpt_1 = pose_3d_model.extract_kpt_2d(heatmap_1, bbox_1, crop_img_1)
    kpt_2 = pose_3d_model.extract_kpt_2d(heatmap_2, bbox_2, crop_img_2)
    kpt_3D = pose_3d_model.calc_3D_kpt(kpt_1, kpt_2)
    kpt_3D.pop('l_eye')
    kpt_3D.pop('r_eye')
    kpt_3D.pop('l_ear')
    kpt_3D.pop('r_ear')
    posString =str(list(kpt_3D.values())).replace("], ", "];")
    sock.sendall(posString.encode("UTF-8"))
    new= time.time()
    fps = 1/(new-prev)
    print(fps)
    prev=new
    if cv2.waitKey(1) & 0xFF==ord('q'):
        break
    cv2.waitKey(1)
cap_1.release()
cap_2.release()
cv2.destroyAllWindows()