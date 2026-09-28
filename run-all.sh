#!/bin/bash
cd /home/kh4idev/att || exit 1
echo "##################### BT01: AES (DES/AES) #####################"
python3 bt01_des-aes/aes.py
echo
echo "##################### BT02: RSA - sinh cap khoa #####################"
python3 bt02_rsa-keygen/rsa_keygen.py
echo
echo "##################### BT03: 3 mo hinh RSA #####################"
( cd bt03_rsa-models-hybrid && python3 rsa_models.py )
echo
echo "##################### BT03: Hybrid RSA+AES #####################"
( cd bt03_rsa-models-hybrid && python3 hybrid_rsa_aes.py )
echo
echo "##################### BT03: So sanh RSA vs AES #####################"
( cd bt03_rsa-models-hybrid && python3 bench_rsa_vs_aes.py )
