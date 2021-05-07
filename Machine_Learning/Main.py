import pandas as pd
import datetime
import csv
import os
import boto3

from Functions import jobPredictor
from Functions import preTraitement

s3 = boto3.resource('s3')
bucket = s3.Bucket('predict-data')

while(True)
    data_list = []
    data_list.append(['OriginalId','Text','Category','CategoryPredict'])
    directories = os.listdir('data')
    for directory in directories:
        if os.path.isdir('data/'+ directory) :
            files = os.listdir('data/'+ directory)
            for file in files:
                print('Reading JSON file...')
                with open('data/'+ directory + '/' + file, newline='') as jsonfile:
                        readjson = pd.read_json(jsonfile)
                        with open('data/label.csv', newline='') as csvfile:
                            readcsv = pd.read_csv(csvfile)
                            i = 0
                            number_lines = len(readjson)
                            while i < number_lines:
                                usefull_text = preTraitement(readjson.description.iloc[i])
                                jobId = jobPredictor(usefull_text)
                                data_list.append([readjson.Id.iloc[i],usefull_text,readcsv['Category'][i],jobId[0]])
                                i = i+1
                print('Creating response...')
                filename = 'predict_' + file + '_' + directory + '.csv'
                with open('data/' + directory + '/' + filename, 'w', newline='') as new_file:
                    writer = csv.writer(new_file, delimiter='|')
                    writer.writerows(data_list)
                #bucket.upload_file('data/' + directory + filename, filename)
                os.remove('data/'+ directory + '/' + file)
                os.remove('data/' + directory + filename)
            os.rmdir('data/' + directory)
