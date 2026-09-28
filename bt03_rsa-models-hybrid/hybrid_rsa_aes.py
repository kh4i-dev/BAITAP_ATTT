"""
MA HOA LAI GHEP RSA + AES (hybrid encryption)
  - Du lieu dai  -> ma hoa bang AES (nhanh)
  - Khoa AES     -> ma hoa bang RSA (khoa cong khai cua nguoi nhan)
Giup tan dung toc do cua AES va tinh bao mat trao khoa cua RSA.
"""
import os
from rsa_util import gen_keypair
from aes import cbc_encrypt, cbc_decrypt

print("Dang sinh khoa RSA-2048...")
pub_r, priv_r = gen_keypair(2048)
e, n = pub_r
d, _ = priv_r

data = ("Day la van ban dai can gui an toan. " * 25).encode()
print("Kich thuoc ban ro:", len(data), "bytes")

# 1) Sinh khoa phien AES ngau nhien
K = os.urandom(16)
iv = os.urandom(16)

# 2) Ma hoa du lieu bang AES
ciphertext = cbc_encrypt(data, K, iv)

# 3) Ma hoa khoa AES bang RSA (khoa cong khai nguoi nhan)
key_int = int.from_bytes(K, "big")
enc_key = pow(key_int, e, n)

print("\n--- GOI TIN GUI DI (hybrid) ---")
print("Khoa phien AES (bi mat)  :", K.hex())
print("Khoa AES da ma hoa (RSA) :", str(enc_key)[:44], "...")
print("IV                       :", iv.hex())
print("Ban ma AES (rut gon)     :", ciphertext[:32].hex(), "...")
print("Tong kich thuoc goi tin  :", len(ciphertext) + 256, "bytes (ban ma + khoa RSA)")

# 4) Nguoi nhan: giai ma khoa AES bang RSA, roi giai ma du lieu bang AES
K2 = pow(enc_key, d, n).to_bytes(16, "big")
plain = cbc_decrypt(ciphertext, K2, iv)
print("\n--- NGUOI NHAN GIAI MA ---")
print("Khoa AES khoi phuc       :", K2.hex(), "->", "khop" if K2 == K else "sai")
print("Ban ro khoi phuc (dau)   :", plain[:40].decode(errors="replace"), "...")
print("Ket qua                  :", "OK" if plain == data else "FAIL")
