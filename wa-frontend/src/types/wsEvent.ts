import { Conversation } from "./chat";

export enum EventType {
    NEW_MESSAGE   = "new_message",
    UPDATE_STATUS = "status_update",
    CONVERSATION_UPDATE = "conversation_update",
    PHONE_NUMBER_UPDATE = "phone_number_update",
    USER_TYPING = "user_typing"
}

export interface WebsocketEvent{
    event: EventType;
    payload: PayloadNewMessage | StatusUpdatePayload | PayloadUserTyping | PayloadConversationUpdate | PayloadPhoneNumberUpdate[];
}

export interface PayloadUserTyping{
    conversation_id: string;
    sender_name: string;
}

export interface PayloadNewMessage{
    message_id: string;
    to: string;
    content: string;
    status: string;
}

export interface StatusUpdatePayload{
    message_id: string;
    wamid : string;
    status: string;
    error_message ?: string;
}

export interface PayloadConversationUpdate{
    id : string;
    phone_number_id:string;
    wa_id: string;
    profile_name: string;
    custom_name: string;
    last_message_at: string;
    last_message_preview: string;
    unread_count: number;
    display_name: string;
    display_phone_number: string;
    is_template_required: boolean;
}

export interface PayloadPhoneNumberUpdate{
    id: string ;
    display_name: string;
    display_phone_number: string;
    unread_count: number;
}