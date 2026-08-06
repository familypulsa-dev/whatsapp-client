import { useState, useEffect, useRef } from 'react';
import { useWS } from '../../stores/ws';
import type { PhoneNumber } from '../../types/chat';
import { EventType, WebsocketEvent } from '@/types/wsEvent';
import { useAuth } from '@/stores/auth';

export type ConnectionStatus = 'connecting' | 'connected' | 'reconnecting' | 'disconnected';

class WebMetaEventEmitter {
    private listeners: Record<string, Function[]> = {};

    public on(event: string, callback: Function) {
        if (!this.listeners[event]) {
            this.listeners[event] = [];
        }
        this.listeners[event].push(callback);
    }

    public off(event: string, callback: Function) {
        if (!this.listeners[event]) return;
        this.listeners[event] = this.listeners[event].filter(cb => cb !== callback);
    }

    public emit(event: string, ...args: any[]) {
        if (!this.listeners[event]) return;
        this.listeners[event].forEach(cb => {
            try {
                cb(...args);
            } catch (e) {
                console.error(`Error in event listener for ${event}:`, e);
            }
        });
    }
}


const decodeBase64Utf8 = (base64Str: string) => {
    try {
        const binString = atob(base64Str);
        const bytes = new Uint8Array(binString.length);
        for (let i = 0; i < binString.length; i++) {
            bytes[i] = binString.charCodeAt(i);
        }
        return new TextDecoder().decode(bytes);
    } catch (error) {
        console.error("Gagal mendecode Base64 ke UTF-8", error);
        return "";
    }
};

export const useChatConnection = () => {
    const emitterRef = useRef<WebMetaEventEmitter>(new WebMetaEventEmitter());
    const { connected, connect, disconnect } = useWS();
      const { token, user} = useAuth()
    const [status, setStatus] = useState<ConnectionStatus>('connecting');

    useEffect(() => {
        setStatus(connected ? 'connected' : 'disconnected');
    }, [connected]);

    useEffect(() => {
        if (token) {
            connect(token, user?.company_id);
        } else {
            setStatus('disconnected');
        }

        // Set the event callback for native WebSocket events
        useWS.setState({
            onEvent: (ev: any) => {
                const wsEvent: WebsocketEvent = ev;
                let parsedPayload: any;

                try {
                    // Handle jika payload berupa string (Base64)
                    if (typeof wsEvent.payload === 'string') {
                        const jsonString = decodeBase64Utf8(wsEvent.payload);
                        parsedPayload = JSON.parse(jsonString);
                    } else {
                        // Fallback jika API sudah mengembalikan object JSON secara langsung
                        parsedPayload = wsEvent.payload;
                    }
                } catch (err) {
                    console.error("Gagal parse payload WS Event", err);
                    return;
                }

                if (wsEvent.event === EventType.NEW_MESSAGE) {
                    emitterRef.current.emit('ReceiveMessage', parsedPayload);
                } else if (wsEvent.event === EventType.UPDATE_STATUS) {
                    console.log('Received status update', parsedPayload);
                    emitterRef.current.emit('MessageStatusUpdated', parsedPayload);
                } else if (wsEvent.event === EventType.CONVERSATION_UPDATE) {
                    emitterRef.current.emit('UpdateConversation', parsedPayload);
                } else if (wsEvent.event === EventType.USER_TYPING) {
                    const { conversation_id, sender_name } = parsedPayload as any;
                    emitterRef.current.emit('UserTyping', conversation_id, sender_name);
                } else if (wsEvent.event === EventType.PHONE_NUMBER_UPDATE) {
                    emitterRef.current.emit('UpdatePhoneNumbers', parsedPayload);
                }
            }
        });

        return () => {
            useWS.setState({ onEvent: null });
            disconnect();
        };
    }, []);

    const handleRetryConnection = () => {
        setStatus('reconnecting');
        const token = localStorage.getItem('token');
        if (token) {
            connect(token, user?.company_id);
        } else {
            window.location.reload();
        }
    };

    const handleFindServer = () => {
        console.log('Finding server...');
    };

    return {
        connectionStatus: status,
        connection: emitterRef.current,
        handleRetryConnection,
        handleFindServer
    };
};
