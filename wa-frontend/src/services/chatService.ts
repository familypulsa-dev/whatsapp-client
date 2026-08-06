import { Contact } from '@/types';
import { get, post, put } from '../api/client';
import type { ApiResponse, PagedResponse, Conversation, PhoneNumber,  Bubble, SendTextResponse, SendTextRequest, SendMediaRequest, SendMediaResponse, SendTemplateRequest, SendTemplateResponse, SendReactionRequest, ConversationResponse, MessageResponse } from '../types/chat';

function qs(params: Record<string, string | number | undefined>): string {
    const sp = new URLSearchParams();
    for (const [k, v] of Object.entries(params)) {
        if (v !== undefined && v !== '') sp.set(k, String(v));
    }
    return sp.toString();
}

export const getConversations = async (
    limit = 50,
    page = 1,
    phone_number_id?: string,
    search?: string,
    filter?: string
): Promise<ApiResponse<ConversationResponse>> => {
    try {
        return await get<ApiResponse<any>>(
            `/api/v1/conversations?${qs({ page, limit, phone_number_id, q: search, filter })}`
        );
    } catch (err: any) {
        return { 
            error: { message: err?.message || 'Gagal mengambil data percakapan' }
        };
    }
};

export const getPingInfo = async () => {
    try {
        await get('/health');
        return { status: true, allowSendTemplate: true };
    } catch (err: any) {
        return { status: false, allowSendTemplate: false };
    }
};

export const getPhoneNumbers = async (): Promise<ApiResponse<PhoneNumber[]>> => {
    try {
        const res = await get<ApiResponse<any[]>>('/api/v1/phone-numbers/active');
        const items: PhoneNumber[] = (res.data || []).map(p => ({
            phone_number_id: p.phone_number_id,
            display_name: p.display_name || p.display_phone_number || 'WA Number',
            unread_count: p.unread_count || 0,
            display_phone_number: p.display_phone_number || '',
        }));
        return { data: items };
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal mengambil ringkasan aplikasi' } };
    }
};

export const getMessages = async (
    conversation_id: string ,
    limit = 30,
    page = 1,
    search?: string,
    message_type?: string,
    direction?: string
): Promise<ApiResponse<MessageResponse>> => {
    try {
        return await get<ApiResponse<any>>(
            `/api/v1/conversations/${conversation_id}/messages?${qs({ limit, page, q: search, type: message_type, direction })}`
        );
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal mengambil pesan' } };
    }
};

export const ensureConversation = async (
    display_phone_number: string,
    phone_number_id: string,
    customer_wa_id: string,
    customer_name?: string
): Promise<ApiResponse<Conversation>> => {
    try {
        let contactData: any = null;
        try {
            const res = await get<ApiResponse<any>>(`/api/v1/contacts/${customer_wa_id}?phone_number_id=${phone_number_id}`);
            contactData = res.data;
        } catch (err: any) { /* ignore */ }

        const conv: Conversation = {
            id: `${phone_number_id}_${customer_wa_id}`,
            phone_number_id,
            wa_id: customer_wa_id,
            custom_name: customer_name || contactData?.company_custom_name || contactData?.profile_name || customer_wa_id,
            display_phone_number: display_phone_number,
            is_template_required: true,
            last_message_preview: '',
            conversation_timestamp: Date.now(),
            unread_count: 0,
            display_name: contactData?.display_name || contactData?.display_phone_number || customer_wa_id,
            last_message_at: new Date().toISOString(),
            profile_name: contactData?.profile_name
        };

        try {
            const listRes = await get<ApiResponse<{ conversations: any[] }>>(`/api/v1/conversations?${qs({ phone_number_id, limit: 100 })}`);
            const existing = (listRes.data?.conversations || []).find((c: any) => c.wa_id === customer_wa_id);
            if (existing) return { data: existing };
        } catch (err: any) { /* ignore */ }

        return { data: conv };
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal membuat percakapan' } };
    }
};

export const markAsRead = async (conversationId: string | number): Promise<ApiResponse<any>> => {
    try {
        return await post<ApiResponse<any>>(`/api/v1/conversations/${conversationId}/read`, {});
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal mark as read' } };
    }
};

export const uploadMedia = async (
    file: File,
    wa_channel_id: string | number
): Promise<ApiResponse<{ media_id: string, file_path: string, file_type: string }>> => {
    try {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('phone_number_id', String(wa_channel_id));

        const res = await post<ApiResponse<any>>('/api/v1/media/upload', formData);
        return {
            data: {
                media_id: res.data?.media_id || res.data?.id || '',
                file_path: `/api/v1/media/${res.data?.media_id || res.data?.id || ''}`,
                file_type: file.type
            }
        };
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal mengunggah media' } };
    }
};

export const sendMessage = async (payload : SendTextRequest): Promise<ApiResponse<SendTextResponse>> => {
    try {
        const res = await post<ApiResponse<SendTextResponse>>('/api/v1/messages/text', payload);
        return { data: res.data };
    } catch (err : Error | any) {
        return { error: { message: err?.message || 'Gagal mengirim pesan' } };
    }
};

export const sendMedia = async (payload : SendMediaRequest): Promise<ApiResponse<any>> => {
    try {
        const formData = new FormData();
        formData.append('to', payload.to);
        formData.append('phone_number_id', payload.phone_number_id);
        formData.append('type', payload.type);
        formData.append('file', payload.file);
        formData.append('body', payload.body);
        formData.append('context_message_id', payload.context_message_id || '');

        const res = await post<ApiResponse<SendMediaResponse>>('/api/v1/messages/media', formData);
        return { data: res?.data };
    } catch (err : Error | any) {
        console.log('error response',err);
        return { error: { message: err?.message || 'Gagal mengirim media' } };
    }
};

export const sendTemplate = async (payload : SendTemplateRequest): Promise<ApiResponse<any>> => {
    try {
        // formData
        const formData = new FormData();
        formData.append('to', payload.to);
        formData.append('phone_number_id', payload.phone_number_id);
        formData.append('type', 'template');
        formData.append('template_name', payload.template_name);
        formData.append('template_lang', payload.template_lang);
        if(payload.template_params && Object.keys(payload.template_params).length > 0) formData.append('template_params', JSON.stringify(payload.template_params));

        const res = await post<ApiResponse<SendTemplateResponse>>('/api/v1/messages/template', formData);
        return { data: res?.data };
    } catch (err : Error | any) {
        return { error: { message: err?.message || 'Gagal mengirim template' } };
    }
};

export const sendReaction = async (payload : SendReactionRequest
): Promise<ApiResponse<any>> => {
    try {
        const res = await post<ApiResponse<any>>('/api/v1/messages/reaction', payload);
        return { data: res.data };
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal mengirim reaksi' } };
    }
};

export const GetContact = async (
    wa_id: string ,
    phone_number_id: string 
):Promise<ApiResponse<any>> => {
    const contactRes = await get<ApiResponse<Contact>>(`/api/v1/contacts?phone_number_id=${phone_number_id}&wa_id=${wa_id}`);
    return contactRes;
}

export const updateConversationName = async (
    phone_number_id: string,
    wa_id: string,
    name: string
): Promise<ApiResponse<any>> => {
    try {
        const res = await put<ApiResponse<any>>(`/api/v1/contacts`, {
            wa_id: wa_id,
            phone_number_id: phone_number_id,
            name: name
        });
        return { data: res.data };
    } catch (err: any) {
        return { error: { message: err?.message || 'Gagal mengubah nama' } };
    }
};

export const sendTypingIndicator = async (
    conversation_id: string | number
): Promise<ApiResponse<any>> => {
    try {
        const res = await post<ApiResponse<any>>(`/api/v1/conversations/${conversation_id}/typing`);
        return { data: res.data };
    } catch (err: any) {
        return { error: { message: err?.message || 'Failed to send typing indicator' } };
    }
};
