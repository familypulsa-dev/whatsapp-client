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
                if (wsEvent.event === EventType.NEW_MESSAGE) {
                    emitterRef.current.emit('ReceiveMessage', wsEvent.payload);
                } else if (wsEvent.event === EventType.UPDATE_STATUS) {
                    emitterRef.current.emit('MessageStatusUpdated', wsEvent.payload);
                } else if (wsEvent.event === EventType.CONVERSATION_UPDATE) {
                    emitterRef.current.emit('UpdateConversation', wsEvent.payload);
                } else if (wsEvent.event === EventType.USER_TYPING) {
                    const { conversation_id, sender_name } = wsEvent.payload as any;
                    emitterRef.current.emit('UserTyping', conversation_id, sender_name);
                } else if (wsEvent.event === EventType.PHONE_NUMBER_UPDATE) {
                    emitterRef.current.emit('UpdatePhoneNumbers', wsEvent.payload);
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
