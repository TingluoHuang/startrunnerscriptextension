#/bin/bash

mkdir /actions-runner

cd /actions-runner

while [ ! -f /actions-runner/.runner ]
do
  echo "Waiting for runner config files..."
  sleep 1
done

ls -l -a .

./run.sh
