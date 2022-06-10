// ==UserScript==
// @name         万华学习题库搜索
// @namespace    http://server.jcdev.cc:52126/
// @license      MIT
// @version      0.1
// @description  使用档案里的题库，对正在答卷的试题进行匹配，对于匹配到的试题给出正确答案
// @author       You
// @match        https://learning.whchem.com:6443/*
// @icon         https://www.google.com/s2/favicons?sz=64&domain=baidu.com
// @connect server.jcdev.cc
// @grant GM_xmlhttpRequest
// @grant GM_log
// @grant GM_addElement
// ==/UserScript==


(function() {
    'use strict';

    function sleep(time) {
        return new Promise(resolve => {
            setTimeout(() => {
                resolve(time);
            }, Math.floor(time * 1000))
        })
    }

    function checkElements(){
        return new Promise((resolve, reject) => {
            let interval = setInterval(() => {
                var element = document.getElementsByClassName('answerQuestion')[0];
                if(element != undefined) {

                    let title = document.getElementById("TimerHolder");
                    if(title == undefined) { return; }

                    sleep(1).then(() => checkElements())
                        .then((element) => appendAnswerLink(element));
                    clearInterval(interval);
                    resolve(element);
                } else {
                    return;
                }

            }, 500);
        });

    }

    function appendAnswerLink(element) {
        GM_log("检测到正在答题");

        let timerNode = document.getElementById('TimerHolder');
        let answerButton = document.getElementById('get_answer');
        if(!answerButton) {
            answerButton = document.createElement('a');
            answerButton.id = 'get_answer';
            answerButton.style = 'margin-left: 3em;';
            answerButton.innerText = '获取答案';
            answerButton.addEventListener('click', () => reportAnswer(element).then());
            timerNode.appendChild(answerButton);
            GM_log("获取答案按钮正确插入");
        }
    }

    function reportAnswer(element){
        return new Promise(resolve => {
            GM_log("获取答案中...");
            var elements = element.getElementsByClassName('swiper-slide');

            elements.forEach(question => {
                let questionItem = question.children[0];
                if(questionItem.id.substring(0,12) !== 'questionItem') {return;}
                let id = questionItem.id.substring(12);
                getAnswer(id).then((data) => fillAnswer(questionItem, data));
            });
        });
    }

    function getAnswer(tmid) {
        return new Promise(resolve => {
            GM_xmlhttpRequest({
                method: "GET",
                url: "http://server.jcdev.cc:52126/get?id=" + tmid,
                responseType: "json",
                onload: function(data) {
                    resolve(data.responseText);
                }
            });
        });

    }

    function fillAnswer(question, data){
        if (data == 'null') return;

        let id = question.id.substring(12);
        let trs = question.getElementsByClassName('question-select-item');
        if(trs.length === 0) {return;}
        let mapping = getAnswerMapping(trs);

        let answers = JSON.parse(data)["Answers"].split(';');
        let newAnswers = [];
        answers.forEach(answer => {
            newAnswers.push(mapping[answer]);
        });
        newAnswers.sort();

        GM_log(newAnswers.join(';') + "/" + JSON.parse(data)["Answers"] + "/" + id);

        let p = document.createElement('p');
        p.style.color = "#018AF4";
        p.innerText = "正确答案：" + newAnswers.join(';');
        // let div = document.createElement('div');
        // div.className = "question-item-right-answer";
        // div.style.marginBottom = "0.5rem";
        question.getElementsByClassName("question-item-right-answer")[0].appendChild(p);
        //div.appendChild(p);
        //question.appendChild(div);

    }

    function getAnswerMapping(answers) {
        let mapping = {};
        answers.forEach(answer => {
            let ori = answer.getAttribute('rel_label');
            let map = answer.getElementsByClassName('question-select-item-label')[0].innerText;
            mapping[ori] = map;
        });

        return mapping;
    }

    sleep(1).then(() => checkElements())
        .then((element) => appendAnswerLink(element));
})();