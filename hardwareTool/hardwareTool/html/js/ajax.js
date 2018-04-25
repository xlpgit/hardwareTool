function ajax(url, onsuccess)   //onsuccess为指向function的变量
{
    var xmlhttp = window.XMLHttpRequest ? new XMLHttpRequest() : new ActiveXObject('Microsoft.XMLHTTP'); //创建XMLHTTP对象，考虑兼容性。XHR
    xmlhttp.open("POST", url, true); //“准备”向服务器的GetDate1.ashx发出Post请求（GET可能会有缓存问题）。这里还没有发出请求    TRUE表示发出异步请求

    //DRY：不要复制粘贴代码
    //AJAX是异步的，并不是等到服务器端返回才继续执行  所以它会跳过onreadystatechange这一段   onreadystatechange这个事件发生才返回
    xmlhttp.onreadystatechange = function ()
    {
        if (xmlhttp.readyState == 4) //readyState == 4 表示服务器返回完成数据了。之前可能会经历2（请求已发送，正在处理中）、3（响应中已有部分数据可用了，但是服务器还没有完成响应的生成）
        {
            if (xmlhttp.status == 200) //如果Http状态码为200则是成功
            {
                onsuccess(xmlhttp.responseText);     //xmlhttp.responseText拿到服务器端返回的数据   即后台的context.respone.write(xmlhttp.responseText);
            }
            else
            {
                alert("AJAX服务器返回错误！");
            }
        }
    }
    //不要以为if (xmlhttp.readyState == 4) {在send之前执行！！！！
    xmlhttp.send(); //这时才开始发送请求。并不等于服务器端返回。请求发出去了，我不等！去监听onreadystatechange吧！
}

function ajax2(url) {
    var xmlhttp = window.XMLHttpRequest ? new XMLHttpRequest() : new ActiveXObject('Microsoft.XMLHTTP'); //创建XMLHTTP对象，考虑兼容性。XHR
    xmlhttp.open("POST", url, true); //“准备”向服务器的GetDate1.ashx发出Post请求（GET可能会有缓存问题）。这里还没有发出请求

    //DRY：不要复制粘贴代码
    //AJAX是异步的，并不是等到服务器端返回才继续执行
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) //readyState == 4 表示服务器返回完成数据了。之前可能会经历2（请求已发送，正在处理中）、3（响应中已有部分数据可用了，但是服务器还没有完成响应的生成）
        {
            if (xmlhttp.status == 200) //如果Http状态码为200则是成功   成功之后进行的操作
            {
                

            }
            else {
                alert("AJAX服务器返回错误！");
            }
        }
    }
    //不要以为if (xmlhttp.readyState == 4) {在send之前执行！！！！
    xmlhttp.send(); //这时才开始发送请求。并不等于服务器端返回。请求发出去了，我不等！去监听onreadystatechange吧！
}