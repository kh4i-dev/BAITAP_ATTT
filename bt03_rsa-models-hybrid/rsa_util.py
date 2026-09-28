"""Tien ich RSA dung chung cho cac bai (sinh khoa, ky, xac thuc)."""
import random
import math
import hashlib

SMALL = [2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47]


def is_probable_prime(n, rounds=30):
    if n < 2:
        return False
    for p in SMALL:
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
        p = random.getrandbits(bits) | (1 << (bits - 1)) | 1
        if is_probable_prime(p):
            return p


def egcd(a, b):
    if b == 0:
        return a, 1, 0
    g, x, y = egcd(b, a % b)
    return g, y, x - (a // b) * y


def modinv(a, m):
    g, x, _ = egcd(a, m)
    if g != 1:
        raise ValueError("khong co nghich dao")
    return x % m


def gen_keypair(bits=1024, e=65537):
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
    return (e, n), (d, n)


def hash_int(data, n):
    """Bam SHA-256 -> so nguyen < n (dai dien thong diep)."""
    return int.from_bytes(hashlib.sha256(data).digest(), "big") % n


def sign(data, private):
    """KY: ma hoa bam bang KHOA BI MAT cua nguoi gui."""
    d, n = private
    return pow(hash_int(data, n), d, n)


def verify(data, signature, public):
    """XAC THUC: giai ma chu ky bang KHOA CONG KHAI cua nguoi gui, so bam."""
    e, n = public
    return pow(signature, e, n) == hash_int(data, n)
