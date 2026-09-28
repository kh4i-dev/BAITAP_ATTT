"""
RSA: sinh cap khoa (bi mat / cong khai) tu dau - khong dung thu vien.
- Sinh so nguyen to lon bang Miller-Rabin
- Tinh n = p*q, phi = (p-1)(q-1), chon e, tinh d = e^-1 mod phi
"""
import random
import math

SMALL_PRIMES = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47]


def is_probable_prime(n, rounds=40):
    if n < 2:
        return False
    for p in SMALL_PRIMES:
        if n % p == 0:
            return n == p
    d, r = n - 1, 0
    while d % 2 == 0:
        d //= 2
        r += 1
    for _ in range(rounds):
        a = random.randrange(2, n - 1)
        x = pow(a, d, n)
        if x in (1, n - 1):
            continue
        for _ in range(r - 1):
            x = pow(x, 2, n)
            if x == n - 1:
                break
        else:
            return False
    return True


def gen_prime(bits):
    while True:
        p = random.getrandbits(bits)
        p |= (1 << (bits - 1)) | 1          # bat bit cao nhat va bit le
        if is_probable_prime(p):
            return p


def modinv(a, m):
    g, x, _ = egcd(a, m)
    if g != 1:
        raise ValueError("khong co nghich dao modulo")
    return x % m


def egcd(a, b):
    if b == 0:
        return a, 1, 0
    g, x, y = egcd(b, a % b)
    return g, y, x - (a // b) * y


def gen_keypair(bits=2048, e=65537):
    while True:
        p = gen_prime(bits // 2)
        q = gen_prime(bits // 2)
        if p == q:
            continue
        phi = (p - 1) * (q - 1)
        if math.gcd(e, phi) == 1:
            break
    n = p * q
    d = modinv(e, phi)
    private = (d, n)                 # khoa bi mat
    public = (e, n)                  # khoa cong khai
    return public, private, p, q, phi


def rsa_encrypt(m, public):
    e, n = public
    return pow(m, e, n)


def rsa_decrypt(c, private):
    d, n = private
    return pow(c, d, n)


if __name__ == "__main__":
    bits = 2048
    pub, priv, p, q, phi = gen_keypair(bits)
    e, n = pub
    d, _ = priv
    print("=== SINH CAP KHOA RSA-%d ===" % bits)
    print("p (bi mat, so nguyen to):", str(p)[:40], "...")
    print("q (bi mat, so nguyen to):", str(q)[:40], "...")
    print("n = p*q (cong khai)      :", str(n)[:40], "...")
    print("phi(n) = (p-1)(q-1)      :", str(phi)[:40], "...")
    print("e (cong khai)            :", e)
    print("d = e^-1 mod phi         :", str(d)[:40], "...")
    print("-> PUBLIC (e,n) , PRIVATE (d,n)")

    m = 123456789
    c = rsa_encrypt(m, pub)
    back = rsa_decrypt(c, priv)
    print("\n=== THU MA HOA / GIAI MA ===")
    print("ban ro   m :", m)
    print("ban ma   c :", str(c)[:50], "...")
    print("giai ma  m':", back, "->", "OK" if back == m else "FAIL")
