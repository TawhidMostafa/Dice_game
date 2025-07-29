# 🎲 Non-Transitive Dice Game (C#)

A console-based C# app that simulates a non-transitive dice game with cryptographic fairness using HMAC.

## 🔹 Features

- Accepts 3+ dice via console (6 comma-separated integers per die)
- Secure HMAC-SHA256 proof for:
  - Coin toss (who starts)
  - Computer's dice & roll
- Menu-based CLI: choose dice, get help, or exit
- `help` shows win probabilities between dice
- Clean error handling, supports multiple rounds

## 💡 Example Input

2,2,4,4,9,9
1,1,6,6,8,8
3,3,5,5,7,7


## 📽️ Submission Demo

Show:
- Launch with 4 identical & 3 custom dice
- Launch with invalid input
- Help screen with probabilities
- Full game rounds with HMAC reveal

## ⚙️ Tech

- C#, .NET
- HMACSHA256
- RandomNumberGenerator
