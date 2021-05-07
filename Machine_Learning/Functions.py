from nltk.tokenize import word_tokenize
from nltk.corpus import stopwords
from nltk.corpus import wordnet
from sklearn.feature_extraction.text import TfidfVectorizer
from nltk.stem import WordNetLemmatizer
from nltk.stem.snowball import EnglishStemmer
import pandas as pd
import pickle
import numpy as np
import re
import nltk


def stopWord(text):
    # Normalisation
    text = text.lower()
    # Suppression de la ponctuation
    text = re.sub(r'[^\w\s]', '', text)
    # Suppression des nombres
    text = re.sub(r"\d+", "", text)
    # Suppression des mots redondants sans sens
    stop_words = set(stopwords.words('english'))
    word_tokens = word_tokenize(text)
    filtered_sentence = [w for w in word_tokens if not w in stop_words]
    return filtered_sentence

def get_wordnet_pos(treebank_tag):

    if treebank_tag.startswith('J'):
        return wordnet.ADJ
    elif treebank_tag.startswith('V'):
        return wordnet.VERB
    elif treebank_tag.startswith('N'):
        return wordnet.NOUN
    elif treebank_tag.startswith('R'):
        return wordnet.ADV
    else:
        return ''

# lemmatization
def lemmatization(text):
    lemmatizer = WordNetLemmatizer()
    res_lemmatizer = []
    tags = nltk.pos_tag(text)
    for word, tag in tags:
        if(get_wordnet_pos(tag) != ''):
            res_lemmatizer.append(lemmatizer.lemmatize(
                word, pos=get_wordnet_pos(tag)))
        else:
            res_lemmatizer.append(lemmatizer.lemmatize(word))
    res_lemmatizer = ' '.join(res_lemmatizer)
    return res_lemmatizer

def preTraitement(text):
    filtered_text = stopWord(text)
    lemmatized_text = lemmatization(filtered_text)
    
    return lemmatized_text
    

def jobPredictor(text):    
    # recupere tfidf
    tfidf = pd.read_pickle("tfidf.pickle")
    tfidf_vector = tfidf.transform([text])

    # recupere modele sgdc
    sgdc = pd.read_pickle("prediction_model.pickle")
    result = sgdc.predict(tfidf_vector)

    return result
