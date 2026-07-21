export interface Template {
   id ?:string,
   waba_id ?: string,
   name : string,
   category : 'UTILITY' | 'AUTHENTICATION' | 'MARKETING',
   message_send_ttl_seconds ?: number,
   language : string,
   status ?: 'PENDING' | 'APPROVED' | 'REJECTED',
   parameter_format ?: 'POSITIONAL' | 'NAMED',
   component ?: Component[],
   meta_template_id ?: string
}

export interface TextComponent extends Component{
    type : 'HEADER' | 'BODY' | 'FOOTER',
    format : string,
    text : string
    example ?: ExampleComponent
}

export interface ExampleComponent {
    body_text : string[],
    header_text : string[],
    footer_text : string[]
}

export interface ImageComponent extends Component{
    type : 'HEADER',
    format : 'IMAGE',
}

export interface ButtonComponent extends Component{
    type : 'BUTTONS',
    buttons : SubButtonComponent[],
}

export type ButtonType = 'url' | 'phone_number' | 'copy_code' | 'otp';

export interface SubButtonComponent{
    url ?: string,
    text : string,
    type : ButtonType,
    otp_type ?: string
}

export interface Component{
    type : 'HEADER' | 'BODY' | 'FOOTER' | 'BUTTONS'
}

