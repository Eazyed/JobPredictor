#!/usr/bin/expect
timestamp=$(date +%Y-%m-%d_%H-%M-%S)
echo "Execution timestamp:" $timestamp
echo "Connecting via SSH to the SandBox VM"
ssh root@localhost -p 2222 << EOF
	echo "Retrieving data from hadoop cluster"
	hdfs dfs -get /user/admin/ResumeAnalyzer ./ResumeAnalyzer$timestamp
EOF
echo "Connection to VM closed"
sleep 1
scp -r -P 2222 root@localhost:/root/ResumeAnalyzer$timestamp $timestamp
ssh -i key.pem ec2-user@ec2-3-89-121-123.compute-1.amazonaws.com << EOF
	echo "IN THE CLOUDS"
	mkdir -p /home/ec2-user/data/$timestamp
EOF
scp -r -i key.pem $timestamp ec2-user@ec2-3-89-121-123.compute-1.amazonaws.com:/home/ec2-user/data
echo "Data sent to cloud !"
sleep 15


