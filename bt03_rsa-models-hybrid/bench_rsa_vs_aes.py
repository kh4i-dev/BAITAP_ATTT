"""
So sanh THOI GIAN ma hoa/giai ma giua RSA-2048 va AES-128.

Luu y: bt01/aes.py la ban AES thuan Python (de HOC thuat toan -> rat cham),
nen o day dung thu vien chuan pycryptodome (ban C, nhanh) de so sanh cong bang.
"""
import os
import time
from rsa_util import gen_keypair
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
from Crypto.Random import get_random_bytes


def aes_enc(data, key, iv):
    return AES.new(key, AES.MODE_CBC, iv).encrypt(pad(data, 16))


def aes_dec(ct, key, iv):
    return unpad(AES.new(key, AES.MODE_CBC, iv).decrypt(ct), 16)


pub, priv = gen_keypair(2048)
e, n = pub
d, _ = priv
key = get_random_bytes(16)
iv = get_random_bytes(16)

print("===== AES-128-CBC (pycryptodome) =====")
for size in (1024, 10240, 1024 * 1024):
    data = get_random_bytes(size)
    t = time.perf_counter()
    ct = aes_enc(data, key, iv)
    te = time.perf_counter() - t
    t = time.perf_counter()
    aes_dec(ct, key, iv)
    td = time.perf_counter() - t
    print(f"  {size:>8} bytes : enc {te*1000:9.3f} ms ({size/te/1e6:7.1f} MB/s)"
          f" | dec {td*1000:9.3f} ms ({size/td/1e6:7.1f} MB/s)")

print("\n===== RSA-2048 (1 khoi nho) =====")
m = int.from_bytes(os.urandom(32), "big") % n
reps = 500
t = time.perf_counter()
for _ in range(reps):
    c = pow(m, e, n)
te = (time.perf_counter() - t) / reps
t = time.perf_counter()
for _ in range(reps):
    pow(c, d, n)
td = (time.perf_counter() - t) / reps
print(f"  ma hoa  : {te*1e6:9.1f} us/lan")
print(f"  giai ma : {td*1e6:9.1f} us/lan")

print("\n===== KET LUAN =====")
print("  - AES: toc do hang tram MB/s -> dung cho DU LIEU LON.")
print(f"  - RSA: ~{te*1e6:.0f}us (ma hoa) / ~{td*1e6:.0f}us (giai ma) cho 1 khoi nho,")
print("         chi ma hoa duoc khoi <= kich thuoc n -> dung cho KHOA/CHU KY.")
print("  => Ket hop: ma hoa du lieu bang AES, trao khoa AES bang RSA (hybrid).")
