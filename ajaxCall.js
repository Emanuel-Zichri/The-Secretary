function ajaxCall(method, api, data, successCB, errorCB) {
    console.log('🚀 ajaxCall נקרא עם:', {
        method: method,
        api: api,
        data: data,
        dataType: typeof data,
        dataLength: data ? data.length : 0
    });

    // בדיקת תקינות JSON
    if (data && method !== 'GET') {
        try {
            JSON.parse(data);
            console.log('✅ JSON תקין');
        } catch (e) {
            console.error('❌ JSON לא תקין:', e);
            console.error('❌ הנתונים שנשלחו:', data);
        }
    }

    $.ajax({
        type: method,
        url: api,
        data: data,
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        beforeSend: function(xhr, settings) {
            console.log('📤 שולח בקשה:', {
                method: settings.type,
                url: settings.url,
                contentType: settings.contentType,
                data: settings.data
            });
        },
        success: function(response, textStatus, xhr) {
            console.log('✅ התקבלה תגובה:', {
                status: xhr.status,
                statusText: xhr.statusText,
                response: response
            });
            if (successCB) successCB(response);
        },
        error: function(xhr, textStatus, errorThrown) {
            console.error('❌ שגיאה בבקשה:', {
                status: xhr.status,
                statusText: xhr.statusText,
                textStatus: textStatus,
                errorThrown: errorThrown,
                responseText: xhr.responseText
            });
            if (errorCB) errorCB(xhr, textStatus, errorThrown);
        }
    });
}

