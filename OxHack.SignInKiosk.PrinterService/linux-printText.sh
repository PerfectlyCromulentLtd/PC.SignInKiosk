#!/bin/bash
echo $1 | base64 --decode | lp -o cpi=12 -o lpi=8