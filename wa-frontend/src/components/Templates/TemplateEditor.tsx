import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ChevronLeft, Phone, Copy, Info } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { get } from '../../api/client';
import type { Template, TextComponent, ImageComponent, ButtonComponent, SubButtonComponent, Component, ExampleComponent } from '../../types/template';

const statusColor = (status?: string) => {
  switch (status?.toUpperCase()) {
    case 'APPROVED': return 'bg-green-100 text-green-800 border-green-200';
    case 'PENDING': return 'bg-yellow-100 text-yellow-800 border-yellow-200';
    case 'REJECTED': return 'bg-red-100 text-red-800 border-red-200';
    default: return 'bg-gray-100 text-gray-800 border-gray-200';
  }
};

const isTextComp = (c: Component): c is TextComponent => 'text' in c;
const isImageComp = (c: Component): c is ImageComponent => 'format' in c && (c as ImageComponent).format === 'IMAGE';
const isButtonComp = (c: Component): c is ButtonComponent => c.type === 'BUTTONS';

function replaceWithExample(text: string, comp: TextComponent | undefined): string {
  if (!comp?.example) return text;
  const example: ExampleComponent = comp.example;
  const exampleKey = `${comp.type.toLowerCase()}_text` as keyof ExampleComponent;
  const values = example[exampleKey] || example[Object.keys(example)[0] as keyof ExampleComponent] || [];
  let idx = 0;
  return text.replace(/\{\{(\d+)\}\}/g, () => {
    const val = values[idx];
    idx++;
    return val ? `<span class="bg-blue-100 text-blue-700 font-bold px-1 rounded mx-0.5">${val}</span>` : `{{${idx}}}`;
  });
}

export default function TemplateEditor() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [template, setTemplate] = useState<Template | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    get<{ success: boolean; data: Template }>(`/api/v1/templates/${id}`)
      .then(res => setTemplate(res.data))
      .catch(err => setError(err.message || 'Failed to load template'))
      .finally(() => setLoading(false));
  }, [id]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen bg-[#f0f2f5]">
        <div className="text-slate-400 animate-pulse">Loading template...</div>
      </div>
    );
  }

  if (error || !template) {
    return (
      <div className="flex items-center justify-center h-screen bg-[#f0f2f5]">
        <div className="text-red-500 text-sm">{error || 'Template not found'}</div>
      </div>
    );
  }

  const rawComponents = ((): Component[] => {
    const raw = (template as any).components || template.component;
    if (typeof raw === 'string') {
      try { return JSON.parse(raw); } catch { return []; }
    }
    return raw || [];
  })();

  const findComp = <T extends Component>(type: string): T | undefined =>
    rawComponents.find(c => c.type?.toUpperCase() === type) as T | undefined;

  const headerComp = findComp<TextComponent | ImageComponent>('HEADER');
  const bodyComp = findComp<TextComponent>('BODY');
  const footerComp = findComp<TextComponent>('FOOTER');
  const buttonComp = findComp<ButtonComponent>('BUTTONS');

  const headerText = headerComp && isTextComp(headerComp) ? headerComp.text : '';
  const bodyText = bodyComp && isTextComp(bodyComp) ? bodyComp.text : '';
  const footerText = footerComp && isTextComp(footerComp) ? footerComp.text : '';
  const buttons = buttonComp?.buttons || [];

  const previewHeader = headerComp && isTextComp(headerComp) ? replaceWithExample(headerComp.text, headerComp) : '';
  const previewBody = bodyComp && isTextComp(bodyComp) ? replaceWithExample(bodyComp.text, bodyComp) : '';
  const previewFooter = footerComp && isTextComp(footerComp) ? replaceWithExample(footerComp.text, footerComp) : '';

  const timeExpiration = template.message_send_ttl_seconds || 60;
  const isAuth = template.category === 'AUTHENTICATION';

  return (
    <div className="flex flex-col h-screen bg-[#f0f2f5] overflow-hidden">
      {/* Header */}
      <header className="bg-white border-b px-6 py-4 flex items-center justify-between shadow-sm z-10">
        <div className="flex items-center gap-4">
          <Button variant="ghost" size="icon" onClick={() => navigate(-1)} className="rounded-full">
            <ChevronLeft className="h-5 w-5" />
          </Button>
          <div>
            <h1 className="text-base font-bold text-slate-800">{template.name || 'Edit Template'}</h1>
            <div className="flex items-center gap-2 mt-0.5">
              <Badge variant="outline" className={`text-[9px] uppercase font-bold tracking-tighter shadow-none border-none py-0 ${statusColor(template.status)}`}>
                {template.status || 'UNKNOWN'}
              </Badge>
              <span className="text-[10px] text-slate-400 uppercase font-medium tracking-wider">{template.category?.replace(/_/g, ' ')}</span>
              <span className="text-[10px] text-slate-400">•</span>
              <span className="text-[10px] text-slate-400">{template.language}</span>
            </div>
          </div>
        </div>
        <Button variant="outline" size="sm" onClick={() => navigate(-1)}>Cancel</Button>
      </header>

      <main className="flex-1 flex overflow-hidden">
        {/* Left Panel: Read-only Form */}
        <div className="w-[450px] bg-white border-r flex flex-col shadow-inner">
          <div className="flex-1 overflow-y-auto p-6 space-y-8">
            {/* Section 1: Basic Info */}
            <div className="space-y-4">
              <h3 className="text-[13px] font-semibold text-slate-900 border-l-4 border-[#00a884] pl-3">
                Basic Information
              </h3>
              <div className="space-y-3 pl-4">
                <div className="space-y-1.5">
                  <label className="text-[10px] font-medium text-slate-500 uppercase tracking-wider">Template Name</label>
                  <div className="h-8 flex items-center text-sm text-slate-800 font-medium bg-slate-50 px-3 rounded border border-slate-200">
                    {template.name}
                  </div>
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <div className="space-y-1.5">
                    <label className="text-[10px] font-medium text-slate-500 uppercase tracking-wider">Category</label>
                    <div className="h-8 flex items-center text-xs bg-slate-50 px-3 rounded border border-slate-200">
                      <Badge variant="outline" className="text-[10px] uppercase font-bold bg-slate-100 text-slate-700 border-slate-200">
                        {template.category?.replace(/_/g, ' ')}
                      </Badge>
                    </div>
                  </div>
                  <div className="space-y-1.5">
                    <label className="text-[10px] font-medium text-slate-500 uppercase tracking-wider">Language</label>
                    <div className="h-8 flex items-center text-xs text-slate-800 bg-slate-50 px-3 rounded border border-slate-200">
                      {template.language}
                    </div>
                  </div>
                </div>
                {template.meta_template_id && (
                  <div className="space-y-1.5">
                    <label className="text-[10px] font-medium text-slate-500 uppercase tracking-wider">Meta Template ID</label>
                    <div className="h-8 flex items-center text-xs text-slate-600 bg-slate-50 px-3 rounded border border-slate-200 font-mono">
                      {template.meta_template_id}
                    </div>
                  </div>
                )}
              </div>
            </div>

            {/* Section 2: Header */}
            {headerComp && (
              <div className="space-y-4">
                <h3 className="text-[13px] font-semibold text-slate-900 border-l-4 border-slate-300 pl-3">
                  Header {isImageComp(headerComp) ? '(Image)' : '(Optional)'}
                </h3>
                <div className="pl-4">
                  {isImageComp(headerComp) ? (
                    <div className="h-24 bg-slate-100 rounded flex items-center justify-center text-slate-400 text-xs border border-slate-200">
                      [Image Header]
                    </div>
                  ) : (
                    <div className="min-h-[32px] text-sm text-slate-800 bg-slate-50 px-3 py-1.5 rounded border border-slate-200 whitespace-pre-wrap break-words">
                      {headerText || <span className="text-slate-300 italic">No header</span>}
                    </div>
                  )}
                </div>
              </div>
            )}

            {/* Section 3: Body */}
            <div className="space-y-4">
              <h3 className="text-[13px] font-semibold text-slate-900 border-l-4 border-[#00a884] pl-3">
                Message Content
              </h3>
              <div className="pl-4">
                <div className="w-full min-h-[80px] p-3 text-sm text-slate-800 bg-slate-50 rounded border border-slate-200 whitespace-pre-wrap break-words">
                  {bodyText || <span className="text-slate-300 italic">No body content</span>}
                </div>
              </div>
            </div>

            {/* Section 4: Auth Settings */}
            {isAuth && (
              <div className="space-y-4">
                <h3 className="text-[13px] font-semibold text-slate-900 border-l-4 border-amber-500 pl-3">
                  Authentication Settings
                </h3>
                <div className="pl-4">
                  <div className="space-y-1.5">
                    <label className="text-[10px] font-medium text-slate-500 uppercase tracking-wider">Code Expiration (Minutes)</label>
                    <div className="h-8 flex items-center text-sm text-slate-800 bg-slate-50 px-3 rounded border border-slate-200">
                      {timeExpiration} minutes
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Section 5: Footer */}
            {footerComp && (
              <div className="space-y-4">
                <h3 className="text-[13px] font-semibold text-slate-900 border-l-4 border-slate-300 pl-3">
                  Footer (Optional)
                </h3>
                <div className="pl-4">
                  <div className="h-8 flex items-center text-sm text-slate-800 bg-slate-50 px-3 rounded border border-slate-200">
                    {footerText || <span className="text-slate-300 italic">No footer</span>}
                  </div>
                </div>
              </div>
            )}

            {/* Section 6: Buttons */}
            {buttons.length > 0 && (
              <div className="space-y-4">
                <h3 className="text-[13px] font-semibold text-slate-900 border-l-4 border-slate-300 pl-3 flex items-center justify-between">
                  Buttons
                  <Badge variant="outline" className="text-[9px] h-4 bg-slate-100 text-slate-600 border-slate-200">{buttons.length}</Badge>
                </h3>
                <div className="pl-4 space-y-2">
                  {buttons.map((btn, idx) => (
                    <div key={idx} className="flex items-center gap-3 bg-slate-50 p-2 rounded-lg border border-slate-100">
                      <div className="w-6 h-6 rounded-full bg-slate-200 text-slate-600 flex items-center justify-center font-bold text-[10px] shrink-0">
                        {btn.type === 'url' ? <Copy className="h-3 w-3" /> :
                         btn.type === 'phone_number' ? <Phone className="h-3 w-3" /> :
                         'OTP'}
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="text-[10px] text-slate-400 uppercase mb-0.5">{btn.type}</div>
                        <div className="text-xs text-slate-800 font-medium">{btn.text}</div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}

            {/* Metadata */}
            {template.message_send_ttl_seconds && (
              <div className="text-[10px] text-slate-400 pt-2 border-t border-slate-100">
                TTL: {template.message_send_ttl_seconds}s
                {template.parameter_format && <> • Format: {template.parameter_format}</>}
              </div>
            )}

            <div className="h-10" />
          </div>
        </div>

        {/* Right Panel: WhatsApp Preview */}
        <div className="flex-1 bg-slate-100 flex items-center justify-center p-8 bg-[url('https://user-images.githubusercontent.com/15075759/28719144-86dc0f70-73b1-11e7-911d-60d70fcded21.png')] bg-repeat">
          <div className="w-full max-w-[480px] bg-white rounded-lg shadow-xl border overflow-hidden">
            <div className="p-6 border-b flex items-center justify-between bg-white">
              <h2 className="text-lg font-bold text-[#142e3e]">Pratinjau template</h2>
              <div className="p-2 border rounded-md hover:bg-slate-50 cursor-pointer">
                <Info className="h-5 w-5 fill-[#142e3e]" />
              </div>
            </div>

            <div className="p-10 bg-[#e5ddd5] bg-[url('https://user-images.githubusercontent.com/15075759/28719144-86dc0f70-73b1-11e7-911d-60d70fcded21.png')] bg-repeat">
              <div className="bg-white rounded-xl shadow-md overflow-hidden relative max-w-[90%]">
                <div className="absolute top-0 left-[-8px] w-0 h-0 border-t-[10px] border-t-white border-l-[10px] border-l-transparent"></div>

                <div className="p-4 space-y-2">
                  {headerComp && isImageComp(headerComp) ? (
                    <div className="w-full h-32 bg-slate-100 rounded flex items-center justify-center text-slate-400 text-xs">
                      [Image Header]
                    </div>
                  ) : previewHeader ? (
                    <h4
                      className="font-bold text-[#111b21] text-base leading-tight break-words"
                      dangerouslySetInnerHTML={{ __html: previewHeader }}
                    />
                  ) : null}

                  <div
                    className="text-[#111b21] text-sm leading-relaxed whitespace-pre-wrap break-words"
                    dangerouslySetInnerHTML={{ __html: previewBody || '<span class="text-slate-300 italic">Body message goes here...</span>' }}
                  />

                  <div className="flex items-end justify-between gap-4 mt-2">
                    {previewFooter ? (
                      <div
                        className="text-[11px] text-[#667781] leading-tight flex-1"
                        dangerouslySetInnerHTML={{ __html: previewFooter }}
                      />
                    ) : <div className="flex-1" />}
                    <span className="text-[10px] text-[#667781] shrink-0">
                      {new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                    </span>
                  </div>
                </div>

                {buttons.length > 0 && (
                  <div className="border-t border-[#f2f2f2] flex flex-col divide-y divide-[#f2f2f2]">
                    {buttons.map((btn, bIdx) => (
                      <div key={bIdx} className="py-2.5 px-3 text-[#00a8e6] font-medium text-center flex items-center justify-center gap-2 hover:bg-slate-50 transition-colors cursor-default">
                        {btn.type === 'phone_number' ? <Phone className="h-3 w-3" /> :
                         btn.type === 'url' ? <Copy className="h-3 w-3" /> :
                         <Info className="h-3 w-3" />}
                        {btn.text}
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  );
}
