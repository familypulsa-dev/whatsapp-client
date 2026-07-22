import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ChevronLeft, Link, MessageSquare, Info, Copy } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { get } from '../../api/client';
import type { Template, TextComponent, ImageComponent, ButtonComponent, SubButtonComponent, Component, ExampleComponent } from '../../types/template';

interface TemplatePreviewProps {
  template?: Template;
  onClose?: () => void;
}

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
    return val ? `<span class="bg-[#00a884] text-blue-700 font-bold px-1 rounded mx-0.5">${val}</span>` : `{{${idx}}}`;
  });
}

export default function TemplatePreview({ template: propTemplate, onClose }: TemplatePreviewProps) {
  const { id } = useParams();
  const navigate = useNavigate();
  const isModal = !!onClose;

  const [template, setTemplate] = useState<Template | null>(propTemplate || null);
  const [loading, setLoading] = useState(!propTemplate && !!id);
  const [error, setError] = useState('');

  useEffect(() => {
    if (propTemplate) {
      setTemplate(propTemplate);
      return;
    }
    if (!id) return;

    setLoading(true);
    get<{ success: boolean; data: Template }>(`/api/v1/templates/${id}`)
      .then(res => setTemplate(res.data))
      .catch(err => setError(err.message || 'Failed to load template'))
      .finally(() => setLoading(false));
  }, [id, propTemplate]);

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

  const headerText = headerComp && isTextComp(headerComp) ? replaceWithExample(headerComp.text, headerComp) : '';
  const bodyText = bodyComp && isTextComp(bodyComp) ? replaceWithExample(bodyComp.text, bodyComp) : '';
  const footerText = footerComp && isTextComp(footerComp) ? replaceWithExample(footerComp.text, footerComp) : '';

  const handleBack = () => {
    if (isModal && onClose) onClose();
    else navigate(-1);
  };

  return (
    <div className="flex flex-col h-screen bg-[#f0f2f5] overflow-hidden">
      {/* Main Preview Area */}
      <main className="flex-1 bg-slate-100 flex items-center justify-center p-8 bg-[url('https://user-images.githubusercontent.com/15075759/28719144-86dc0f70-73b1-11e7-911d-60d70fcded21.png')] bg-repeat">
        <div className="w-full max-w-[480px]">
          {/* Message Bubble */}
          <div className="bg-white rounded-lg shadow-xl overflow-hidden relative max-w-[90%]">
            <div className="absolute top-0 left-[-8px] w-0 h-0 border-t-[10px] border-t-white border-l-[10px] border-l-transparent"></div>

            <div className="p-4 space-y-2">
              {headerComp && isImageComp(headerComp) ? (
                <div className="w-full h-40 bg-slate-100 rounded flex items-center justify-center text-slate-400 text-xs">
                  [Image Header]
                </div>
              ) : headerText ? (
                <h4
                  className="font-bold text-[#111b21] text-base leading-tight break-words"
                  dangerouslySetInnerHTML={{ __html: headerText }}
                />
              ) : null}

              <div
                className="text-[#111b21] text-sm leading-relaxed whitespace-pre-wrap break-words"
                dangerouslySetInnerHTML={{ __html: bodyText || '<span class="text-slate-300 italic">No body content</span>' }}
              />

              <div className="flex items-end justify-between gap-4 mt-2">
                {footerText ? (
                  <div
                    className="text-[11px] text-[#667781] leading-tight flex-1"
                    dangerouslySetInnerHTML={{ __html: footerText }}
                  />
                ) : <div className="flex-1" />}
                <span className="text-[10px] text-[#667781] shrink-0">
                  {new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </span>
              </div>
            </div>

            {/* Buttons */}
            {buttonComp?.buttons?.length > 0 && (
              <div className="border-t border-[#f2f2f2] flex flex-col divide-y divide-[#f2f2f2]">
                {buttonComp.buttons.map((btn: SubButtonComponent, bIdx: number) => (
                  <div key={bIdx} className="py-2.5 px-3 text-[#00a8e6] font-medium text-center flex items-center justify-center gap-2 hover:bg-slate-50 transition-colors cursor-default">
                    {btn.type === 'url' ? <Link className="h-3 w-3" /> :
                     btn.type === 'quick_reply' ? <MessageSquare className="h-3 w-3" /> :
                     btn.type === 'copy_code' ? <Copy className="h-3 w-3" /> :
                     <Info className="h-3 w-3" />}
                    {btn.text}
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Metadata footer */}
          <div className="mt-3 text-[10px] text-slate-400 flex items-center gap-2">
            {template.meta_template_id && (
              <span>Meta ID: {template.meta_template_id}</span>
            )}
            {template.message_send_ttl_seconds && (
              <>
                <span>•</span>
                <span>TTL: {template.message_send_ttl_seconds}s</span>
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
