# https://www.youtube.com/watch?v=U_eV8wfMkXU&ab_channel=GregKamradt%28DataIndy%29

import os
import sys

import streamlit as st
from langchain import PromptTemplate
from langchain.llms import OpenAI

os.environ['OPENAI_API_KEY'] =

#prompt template code
template = """
Below is an email that may be poorly formatted.
Your goal is to:
- Properly format the text
- Convert the input text to a specified tone
- Convert the input text to a specified dialect

Here are some examples of different Tones:
- Formal: We went to Barcelona for the weekend. We have a lot of things to tell
- Informal: Went to Barcelona for the weekend. Lots to tell you. 

Here are some examples of words in different dialects:
- American English: French Fries, Cotton Candy, Apartment, Garbage, Cookie, Green Thumb, Parking Lot, Pants, Windshield
- British English: Chips, Candyfloss, Flag, Rubbish, Biscuit, Green Fingers, Car Park, Trousers, Windscreen

Below is the text, tone and dialect:
TONE: {tone}
DIALECT: {dialect}
Text: {text}

YOUR RESPONSE

"""

prompt = PromptTemplate(
    input_variables=["tone","dialect","text"],
    template=template,
)

def load_LLM():
    """Logic for loading the chain to be used"""
    llm = OpenAI(temparature=.5, verbose=True)
    return llm 

llm = load_LLM()

st.set_page_config(page_title="Globalize Email", page_icon=":robot:")
st.header("LangChain for ML Agents")

col1, col2, col3 = st.columns(3)

with col1:
    st.markdown("### What would you like to learn about today?")
    st.write("I am testing this LangChain transformer.")

with col2:
    st.markdown("### Dialect Converter ")
    st.write("[Github](https://github.com/icecoldt369)")

with col3:
    st.image(image='image.png', width=400, caption='This is a beta website template for the transformer sequence.')

st.markdown("## Enter Your Question to Query")

col1, col2 = st.columns(2)

with col1:
    option_tone = st.selectbox(
        "Which tone would you like to converse with?",
        ("Formal", "Informal")
    )

with col2:
    option_dialect = st.selectbox(
        "How are you expressing the feeling?",
        ("American English", "British English")
    )

def get_text():
    input_text = st.text_area(label="", placeholder="Your Email...", key="email_input")
    return input_text

email_input = get_text()


st.markdown("### Your Converted Text: ")

if email_input:
    prompt_with_email = (prompt.format(tone=option_tone, dialect=option_dialect, email=email_input))
    formatted_email = llm(prompt_with_email)


    st.write(formatted_email)


# can do maths using st.write
#st.write(1+2)
