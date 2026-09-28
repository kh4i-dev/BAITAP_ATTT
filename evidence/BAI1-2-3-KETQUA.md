##################### BT01: AES (DES/AES) #####################
FIPS-197 vector:
  plaintext : 00112233445566778899aabbccddeeff
  key       : 000102030405060708090a0b0c0d0e0f
  ciphertext: 69c4e0d86a7b0430d8cdb78070b4c55a
  expect    : 69c4e0d86a7b0430d8cdb78070b4c55a -> PASS

CBC demo:
  plaintext : Xin chao, day la minh hoa ma hoa AES-128-CBC!
  ciphertext: b1685fbacb72ea57873bab446820c670429b305f5e0922fb745abc8309f6cb76b484db60333a8f9c37aa1ceaaf5e8978
  decrypt   : Xin chao, day la minh hoa ma hoa AES-128-CBC!
  round-trip: OK

##################### BT02: RSA - sinh cap khoa #####################
=== SINH CAP KHOA RSA-2048 ===
p (bi mat, so nguyen to): 1595914745959592922982410566407727069206 ...
q (bi mat, so nguyen to): 1343103242120489176760220739762322780325 ...
n = p*q (cong khai)      : 2143478269446226109778100110073857794494 ...
phi(n) = (p-1)(q-1)      : 2143478269446226109778100110073857794494 ...
e (cong khai)            : 65537
d = e^-1 mod phi         : 7779540430146014350242136383755249935127 ...
-> PUBLIC (e,n) , PRIVATE (d,n)

=== THU MA HOA / GIAI MA ===
ban ro   m : 123456789
ban ma   c : 18784293362165031497958451230254608135209158197293 ...
giai ma  m': 123456789 -> OK

##################### BT03: 3 mo hinh RSA #####################
Dang sinh khoa (co the mat vai giay)...
Khoa NGUOI GUI  : RSA-1024
Khoa NGUOI NHAN : RSA-2048
Thong diep      : Chuyen 1.000.000 VND cho tai khoan 123456

================ MO HINH 1: XAC THUC NGUOI GUI ================
  Nguoi gui KY bang khoa BI MAT cua minh   : sig = 17109748025368414884027324628353411901195509 ...
  Nguoi nhan XAC THUC bang khoa CONG KHAI  : HOP LE
  Thu sua noi dung -> xac thuc             : BI TU CHOI (dung)

================ MO HINH 2: XAC THUC NGUOI NHAN ================
  Nguoi gui MA HOA bang khoa CONG KHAI nguoi nhan
  Ban ma c = 17221724056595689591801101836789871586503257 ...
  Nguoi nhan GIAI MA bang khoa BI MAT cua minh: Chuyen 1.000.000 VND cho tai khoan 123456
  Ket qua: OK (chi nguoi nhan doc duoc)

================ MO HINH 3: CA HAI (KY + MA HOA) ===============
  Buoc 1: nguoi gui KY                  -> sig
  Buoc 2: nguoi gui MA HOA sig (khoa nhan) -> c
  Nguoi nhan GIAI MA -> KY -> XAC THUC  : HOP LE (bao mat + xac thuc)

##################### BT03: Hybrid RSA+AES #####################
Dang sinh khoa RSA-2048...
Kich thuoc ban ro: 900 bytes

--- GOI TIN GUI DI (hybrid) ---
Khoa phien AES (bi mat)  : 350246cf8d967d38e0ea4d6cec71f599
Khoa AES da ma hoa (RSA) : 16606638135974756496843558846450910316801650 ...
IV                       : bc9b442296ba73f405b99529cb3815e9
Ban ma AES (rut gon)     : d17df53dbe46830b167ef73ad27f3c80a2847738c9a9646d7c7918de22400edc ...
Tong kich thuoc goi tin  : 1168 bytes (ban ma + khoa RSA)

--- NGUOI NHAN GIAI MA ---
Khoa AES khoi phuc       : 350246cf8d967d38e0ea4d6cec71f599 -> khop
Ban ro khoi phuc (dau)   : Day la van ban dai can gui an toan. Day  ...
Ket qua                  : OK

##################### BT03: So sanh RSA vs AES #####################
===== AES-128-CBC (pycryptodome) =====
      1024 bytes : enc     1.240 ms (    0.8 MB/s) | dec     0.049 ms (   20.8 MB/s)
     10240 bytes : enc     0.369 ms (   27.7 MB/s) | dec     0.087 ms (  118.1 MB/s)
   1048576 bytes : enc    15.986 ms (   65.6 MB/s) | dec    16.187 ms (   64.8 MB/s)

===== RSA-2048 (1 khoi nho) =====
  ma hoa  :     113.2 us/lan
  giai ma :   22671.0 us/lan

===== KET LUAN =====
  - AES: toc do hang tram MB/s -> dung cho DU LIEU LON.
  - RSA: ~113us (ma hoa) / ~22671us (giai ma) cho 1 khoi nho,
         chi ma hoa duoc khoi <= kich thuoc n -> dung cho KHOA/CHU KY.
  => Ket hop: ma hoa du lieu bang AES, trao khoa AES bang RSA (hybrid).
