"""
3 MO HINH AP DUNG RSA

  Mo hinh 1 - Xac thuc nguoi GUI : nguoi gui KY bang khoa BI MAT cua minh;
                                   nguoi nhan XAC THUC bang khoa CONG KHAI cua nguoi gui.
  Mo hinh 2 - Xac thuc nguoi NHAN: nguoi gui MA HOA bang khoa CONG KHAI cua nguoi nhan;
                                   chi nguoi nhan (co khoa BI MAT) moi giai duoc.
  Mo hinh 3 - CA HAI (ky + ma hoa): bao mat (chi nguoi nhan doc) + xac thuc nguoi gui.
"""
from rsa_util import gen_keypair, sign, verify, hash_int


def b2i(b):
    return int.from_bytes(b, "big")


def i2b(i, length=None):
    if length is None:
        length = max(1, (i.bit_length() + 7) // 8)
    return i.to_bytes(length, "big")


print("Dang sinh khoa (co the mat vai giay)...")
pub_s, priv_s = gen_keypair(1024)     # khoa NGUOI GUI
pub_r, priv_r = gen_keypair(2048)     # khoa NGUOI NHAN (lon hon de chua chu ky)
e_s, n_s = pub_s
d_s, _ = priv_s
e_r, n_r = pub_r
d_r, _ = priv_r
print("Khoa NGUOI GUI  : RSA-%d" % n_s.bit_length())
print("Khoa NGUOI NHAN : RSA-%d" % n_r.bit_length())

msg = "Chuyen 1.000.000 VND cho tai khoan 123456".encode()
print("Thong diep      :", msg.decode())

print("\n================ MO HINH 1: XAC THUC NGUOI GUI ================")
sig = sign(msg, priv_s)
print("  Nguoi gui KY bang khoa BI MAT cua minh   : sig =", str(sig)[:44], "...")
print("  Nguoi nhan XAC THUC bang khoa CONG KHAI  :", "HOP LE" if verify(msg, sig, pub_s) else "SAI")
tampered = msg.replace(b"1.000.000", b"9.000.000")
print("  Thu sua noi dung -> xac thuc             :", "HOP LE (lo hong!)" if verify(tampered, sig, pub_s) else "BI TU CHOI (dung)")

print("\n================ MO HINH 2: XAC THUC NGUOI NHAN ================")
m = b2i(msg)
c = pow(m, e_r, n_r)
back = pow(c, d_r, n_r)
print("  Nguoi gui MA HOA bang khoa CONG KHAI nguoi nhan")
print("  Ban ma c =", str(c)[:44], "...")
print("  Nguoi nhan GIAI MA bang khoa BI MAT cua minh:", i2b(back).decode(errors="replace"))
print("  Ket qua:", "OK (chi nguoi nhan doc duoc)" if back == m else "FAIL")

print("\n================ MO HINH 3: CA HAI (KY + MA HOA) ===============")
sig3 = sign(msg, priv_s)                 # 1) KY bang khoa bi mat nguoi gui
c3 = pow(sig3, e_r, n_r)                 # 2) MA HOA chu ky bang khoa cong khai nguoi nhan
sig3_back = pow(c3, d_r, n_r)            # nguoi nhan: giai ma -> lay chu ky
ok = verify(msg, sig3_back, pub_s)       #           xac thuc bang khoa cong khai nguoi gui
print("  Buoc 1: nguoi gui KY                  -> sig")
print("  Buoc 2: nguoi gui MA HOA sig (khoa nhan) -> c")
print("  Nguoi nhan GIAI MA -> KY -> XAC THUC  :", "HOP LE (bao mat + xac thuc)" if ok else "SAI")
