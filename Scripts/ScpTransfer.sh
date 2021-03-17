#!/usr/bin/expect
ec2Dns=$1
clouderaUser=$2
timestamp=$(date +%Y-%m-%d_%H-%M-%S)
hourstamp=$(date +%H-%M-%S)
echo "Execution timestamp:" $timestamp
echo "Connecting via SSH to the SandBox VM"
ssh $clouderaUser@localhost -p 2222 << EOF 
		echo "Retrieving data from hadoop cluster"
		hdfs dfs -mkdir -p /user/$clouderaUser/data/$(date +%Y)/$(date +%m)/$(date +%d)/$hourstamp
		hdfs dfs -mv /user/$clouderaUser/rawdata/* /user/$clouderaUser/data/$(date +%Y)/$(date +%m)/$(date +%d)/$hourstamp
		hdfs dfs -get /user/$clouderaUser/data/$(date +%Y)/$(date +%m)/$(date +%d)/$hourstamp ./data/ResumeAnalyzer$timestamp
EOF
echo "Connection to Sandbox VM closed"
sleep 1
scp -r -P 2222 $clouderaUser@localhost:/$clouderaUser/data/ResumeAnalyzer$timestamp $timestamp
if [ "$(ls -A $timestamp)" ]; 
	then
	ssh -i key.pem ec2-user@$ec2Dns << EOF
		echo "Connected to the cloud VM"
		mkdir -p /home/ec2-user/data/$timestamp
EOF
	scp -r -i key.pem $timestamp ec2-user@$ec2Dns:/home/ec2-user/data
	echo "Data sent to cloud !"
fi
rm -r $timestamp
echo "End of the script";
sleep 15



