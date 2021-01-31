function removeAccents(str) {
    str = stringToSlug(str);
   var s1 =  str.normalize('NFD')
       .replace(/[`“”"~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi,' ')
        .replace(/đ/g, 'd').replace(/Đ/g, 'D');
    s = s1.split(' ')
    var value = ''
    for (var i = 0; i < s.length; i++) {
        
        if (s[i] != '-'&& s[i]!='') {
            value = value == '' ? s[i]: value + '-' + s[i]
        }
        //s = value;
    }
    return value+".html";
}
