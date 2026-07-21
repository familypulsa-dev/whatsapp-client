import { useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import { ChevronLeft, Phone, Copy, Info, Plus, X, Send, Save } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { post } from '../../api/client';
import type { SubButtonComponent, ButtonType } from '../../types/template';

interface DraftButton {
  id: string;
  text: string;
  type: ButtonType;
  url?: string;
  otp_type?: string;
}

const detectVars = (text: string): number[] =>
  [...new Set([...text.matchAll(/\{\{(\d+)\}\}/g)].map(m => parseInt(m[1])))].sort();

const BUTTON_TYPES: { value: ButtonType; label: string; icon: any }[] = [
  { value: 'url', label: 'URL', icon: Copy },
  { value: 'phone_number', label: 'Phone', icon: Phone },
  { value: 'copy_code', label: 'Copy', icon: Info },
  { value: 'otp', label: 'OTP', icon: Info },
];

function ExampleInputs({ vars, examples, setExamples, color }: {
  vars: number[];
  examples: Record<number, string>;
  setExamples: (fn: (prev: Record<number, string>) => Record<number, string>) => void;
  color: string;
}) {
  if (!vars.length) return null;
  return (
    <div className="space-y-2 mt-2 border-t border-dashed border-slate-200 pt-2">
      {vars.map(v => {
        const label = `{{${v}}}`;
        return (
          <div key={v} className="flex items-center gap-2">
            <span className="text-[10px] font-mono text-slate-400 w-8 shrink-0">{label}</span>
            <input
              className={`flex-1 h-7 px-2 text-[11px] border rounded focus:outline-none focus:ring-1 focus:ring-[#00a884]/20 ${color}`}
              placeholder={`Example for ${label}...`}
              value={examples[v] || ''}
              onChange={e => setExamples(prev => ({ ...prev, [v]: e.target.value }))}
            />
          </div>
        );
      })}
    </div>
  );
}

let idCounter = 0;
const freshId = () => `b${++idCounter}`;

export default function TemplateCreate() {
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [category, setCategory] = useState<'UTILITY' | 'AUTHENTICATION' | 'MARKETING'>('UTILITY');
  const [language, setLanguage] = useState('id');
  const [headerText, setHeaderText] = useState('');
  const [bodyText, setBodyText] = useState('');
  const [footerText, setFooterText] = useState('');
  const [timeExp, setTimeExp] = useState(60);
  const [buttons, setButtons] = useState<DraftButton[]>([]);
  const [examplesBody, setExamplesBody] = useState<Record<number, string>>({});
  const [examplesHeader, setExamplesHeader] = useState<Record<number, string>>({});
  const [isSaving, setIsSaving] = useState(false);

  const isAuth = category === 'AUTHENTICATION';

  const headerVars = useMemo(() => detectVars(headerText), [headerText]);
  const bodyVars = useMemo(() => detectVars(bodyText), [bodyText]);

  useEffect(() => {
    setExamplesBody(prev => {
      const next = { ...prev };
      Object.keys(next).forEach(k => { if (!bodyVars.includes(Number(k))) delete next[Number(k)]; });
      return next;
    })
  }, [bodyVars]);

  useEffect(() => {
    setExamplesHeader(prev => {
      const next = { ...prev };
      Object.keys(next).forEach(k => { if (!headerVars.includes(Number(k))) delete next[Number(k)]; });
      return next;
    })
  }, [headerVars]);

  useEffect(() => {
    if (!isAuth) return;
    setBodyText('*{{1}}* adalah kode verifikasi Anda. Demi keamanan, jangan bagikan kode ini.');
    setButtons([{ id: freshId(), type: 'otp', otp_type: 'copy_code', text: 'Copy Code' }]);
  }, [category]);

  useEffect(() => {
    if (isAuth) setFooterText(`Kedaluwarsa dalam ${timeExp} menit.`);
  }, [timeExp, isAuth]);

  const highlight = (text: string, examples: Record<number, string> = {}) =>
    text.replace(/\{\{(\d+)\}\}/g, (_, n) => {
      const val = examples[parseInt(n)];
      return `<span class="bg-[#00a884] text-white font-bold px-1 rounded mx-0.5">${val || `{{${n}}}`}</span>`;
    });

  const previewHeader = useMemo(() => highlight(headerText,examplesHeader), [headerText, examplesHeader]);
  const previewBody = useMemo(() => highlight(bodyText, examplesBody), [bodyText, examplesBody]);
  const previewFooter = useMemo(() => highlight(footerText), [footerText]);

  const addBtn = (type: ButtonType) =>
    setButtons(prev => [...prev, { id: freshId(), text: '', type, otp_type: type === 'otp' ? 'COPY_CODE' : undefined }]);

  const updBtn = (id: string, patch: Partial<DraftButton>) =>
    setButtons(prev => prev.map(b => b.id === id ? { ...b, ...patch } : b));

  const delBtn = (id: string) =>
    setButtons(prev => prev.filter(b => b.id !== id));

  const handleSave = async () => {
    if (!name.trim()) return alert('Nama template harus diisi');
    if (!bodyText.trim()) return alert('Body template harus diisi');

    setIsSaving(true);
    try {
      const components: any[] = [];

      if(isAuth) {
        components.push({ type: 'BODY' });
      }else{
        if (bodyText.trim()) {
          components.push({ type: 'BODY', text: bodyText });
        }
      }

      if (headerText.trim()) {
        if(isAuth) {
          components.push({ type: 'HEADER' });
        }else{
          components.unshift({ type: 'HEADER', format: 'TEXT', text: headerText });
        }
      }

      if (footerText.trim()) {
        if(isAuth) {
          components.push({ type: 'FOOTER' });
        }else{
          components.push({ type: 'FOOTER', text: footerText });
        }
      }

      // add examples per component
      for (const comp of components) {
        if (comp.type === 'HEADER') {
          const vals = headerVars.map(v => examplesHeader[v] || '');
          if (vals.some(Boolean)) comp.example = { header_text: vals };
        } else if (comp.type === 'BODY') {
          if(isAuth) continue;
          const vals = bodyVars.map(v => examplesBody[v] || '');
          if (vals.some(Boolean)) comp.example = { body_text: [vals] };
        }
      }

      if (buttons.length) {
        components.push({ type: 'BUTTONS', buttons: buttons.map(b => {
          if(b.type === 'otp'){
            const btn: any = { type: b.type};
            if (b.type === 'otp') btn.otp_type = b.otp_type;
            return btn;
          }else{
             const btn: any = { type: b.type, text: b.text };
            if (b.type === 'url') btn.url = b.url;
            if (b.type === 'phone_number') btn.url = b.url;
            return btn;
          }
        }) });
      }

      const res = await post<{ success: boolean; data?: any; error?: string }>('/api/v1/templates', {
        name: name.trim(),
        category,
        language,
        components,
      });

      if (!res.success) {
        alert(res.error || 'Gagal menyimpan template');
        return;
      }

      navigate('/templates');
    } catch (err: any) {
      alert(err?.message || 'Terjadi kesalahan');
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="flex flex-col h-screen bg-[#f0f2f5] overflow-hidden">
      <header className="bg-white border-b px-6 py-4 flex items-center justify-between shadow-sm z-10">
        <div className="flex items-center gap-4">
          <h1 className="text-base font-bold text-slate-800">
            {'Buat Template Baru'}
          </h1>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="ghost" onClick={() => navigate(-1)}>
            Cancel
          </Button>
          <Button onClick={handleSave} disabled={isSaving}>
            <Save className="w-4 h-4 mr-1.5" />
            {isSaving ? 'Menyimpan...' : 'Save'}
          </Button>
        </div>
      </header>

      <main className="flex-1 flex overflow-hidden">
        <div className="w-[420px] bg-white border-r flex flex-col shadow-inner overflow-y-auto p-5 space-y-4">
          {/* Template Name Card */}
          <div className="bg-white border border-slate-200 rounded-lg p-4 space-y-3 shadow-sm">
            <label className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">Template Name</label>
            <input
              className="w-full h-9 px-3 text-sm border rounded-md focus:outline-none focus:ring-2 focus:ring-[#00a884]/20 border-slate-200"
              placeholder="e.g. registration_otp"
              value={name}
              onChange={e => setName(e.target.value.toLowerCase().replace(/[^a-z0-9_]/g, '_'))}
            />
            <div className="grid grid-cols-2 gap-3">
              <select
                className="h-8 px-2 text-xs border rounded-md border-slate-200 bg-white focus:outline-none focus:ring-2 focus:ring-[#00a884]/20"
                value={category}
                onChange={e => setCategory(e.target.value as any)}
              >
                <option value="UTILITY">Utility</option>
                <option value="MARKETING">Marketing</option>
                <option value="AUTHENTICATION">Authentication</option>
              </select>
              <select
                className="h-8 px-2 text-xs border rounded-md border-slate-200 bg-white focus:outline-none focus:ring-2 focus:ring-[#00a884]/20"
                value={language}
                onChange={e => setLanguage(e.target.value)}
              >
                <option value="id">Indonesian</option>
                <option value="en">English</option>
              </select>
            </div>
          </div>

          {/* Content Card */}
          <div className="bg-white border border-slate-200 rounded-lg p-4 space-y-4 shadow-sm">
            <h3 className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Content</h3>

            {/* Header */}
            <div>
              <label className="text-[11px] font-medium text-slate-600">Header / Judul (Optional)</label>
              <input disabled={!isAuth}
                className="w-full h-8 px-3 pb-2 text-xs border rounded-md mt-1 mb-1 focus:outline-none focus:ring-2 focus:ring-[#00a884]/20 border-slate-200"
                placeholder="e.g. Order Status Update"
                value={headerText}
                onChange={e => setHeaderText(e.target.value)}
              />
              {!isAuth && headerVars.length < 1 && (
                <Button
                  variant="secondary"
                  size="sm"
                  className="h-7 text-[10px] px-2 bg-slate-100 mt-1"
                  onClick={() => setHeaderText(prev => prev + `{{${headerVars.length + 1}}}`)}
                >
                  <Plus className="h-3 w-3 mr-1" /> Add Variable
                </Button>
              )}
              <ExampleInputs vars={headerVars} examples={examplesHeader} setExamples={setExamplesHeader} color="border-blue-200 bg-blue-50/30" />
            </div>

            {/* Body */}
            <div>
              <label className="text-[11px] font-medium text-slate-600">Isi / Body</label>
              <textarea
                className="w-full min-h-[80px] p-2 text-xs border rounded-md mt-1 focus:outline-none focus:ring-2 focus:ring-[#00a884]/20 border-slate-200 resize-none bg-slate-50"
                placeholder="Type message here... Use {{1}} for variables."
                value={bodyText}
                onChange={e => setBodyText(e.target.value)}
                disabled={isAuth}
              />
              {!isAuth && (
                <Button
                  variant="secondary"
                  size="sm"
                  className="h-7 text-[10px] px-2 bg-slate-100 mt-1"
                  onClick={() => setBodyText(prev => prev + `{{${bodyVars.length + 1}}}`)}
                >
                  <Plus className="h-3 w-3 mr-1" /> Add Variable
                </Button>
              )}
              <ExampleInputs vars={bodyVars} examples={examplesBody} setExamples={setExamplesBody} color="border-green-200 bg-green-50/30" />
            </div>

            {/* Footer */}
            <div>
              <label className="text-[11px] font-medium text-slate-600">Catatan Kaki / Footer (Optional)</label>
              <input
                className="w-full h-8 px-3 text-xs border rounded-md mt-1 focus:outline-none focus:ring-2 focus:ring-[#00a884]/20 border-slate-200"
                placeholder="e.g. Regards, Family Pulsa"
                value={footerText}
                onChange={e => setFooterText(e.target.value)}
                disabled={isAuth}
              />
            </div>

            {isAuth && (
              <div className="pt-2 border-t border-slate-100">
                <label className="text-[10px] font-medium text-slate-500 uppercase tracking-wider">Code Expiration (Minutes)</label>
                <input
                  type="number"
                  className="w-full h-8 px-3 text-xs border rounded-md mt-1 focus:outline-none focus:ring-2 focus:ring-[#00a884]/20 border-slate-200"
                  value={timeExp}
                  onChange={e => setTimeExp(parseInt(e.target.value) || 1)}
                  min={1} max={60}
                />
              </div>
            )}
          </div>

          {/* Buttons Card */}
          <div className="bg-white border border-slate-200 rounded-lg p-4 space-y-3 shadow-sm">
            <div className="flex items-center justify-between">
              <h3 className="text-[11px] font-bold text-slate-400 uppercase tracking-widest">Buttons</h3>
              <span className="text-[10px] text-slate-400">{buttons.length}</span>
            </div>

            {buttons.map(btn => (
              <div key={btn.id} className="bg-slate-50 p-3 rounded-lg border border-slate-200 space-y-2">
                <div className="flex items-center justify-between">
                  <select
                    className="h-6 text-[10px] border rounded px-1 bg-white border-slate-200 focus:outline-none focus:ring-1 focus:ring-[#00a884]/20"
                    value={btn.type}
                    onChange={e => updBtn(btn.id, { type: e.target.value as ButtonType })}
                    disabled={isAuth}
                  >
                    {BUTTON_TYPES.map(t => <option key={t.value} value={t.value}>{t.label}</option>)}
                  </select>
                  {!isAuth && (
                    <button onClick={() => delBtn(btn.id)} className="text-red-400 hover:text-red-600">
                      <X className="h-3.5 w-3.5" />
                    </button>
                  )}
                </div>
                <input
                  className="w-full h-7 px-2 text-[11px] border rounded border-slate-200 focus:outline-none focus:ring-1 focus:ring-[#00a884]/20 bg-white"
                  placeholder="Button text"
                  value={btn.text}
                  onChange={e => updBtn(btn.id, { text: e.target.value })}
                />
                {btn.type === 'url' && (
                  <input
                    className="w-full h-7 px-2 text-[11px] border rounded border-slate-200 focus:outline-none focus:ring-1 focus:ring-[#00a884]/20 bg-white"
                    placeholder="https://example.com"
                    value={btn.url || ''}
                    onChange={e => updBtn(btn.id, { url: e.target.value })}
                  />
                )}
              </div>
            ))}

            {!isAuth && (
              <div className="flex flex-wrap gap-2">
                {BUTTON_TYPES.map(t => (
                  <Button
                    key={t.value}
                    variant="outline"
                    size="sm"
                    className="h-7 text-[10px] border-slate-300 text-slate-600"
                    onClick={() => addBtn(t.value)}
                  >
                    <t.icon className="h-3 w-3 mr-1" /> {t.label}
                  </Button>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Preview Panel */}
        <div className="flex-1 bg-slate-100 flex items-center justify-center p-8 ">
          <div className="w-full max-w-[480px] bg-white rounded-lg shadow-xl border overflow-hidden">
            <div className="p-6 border-b flex items-center justify-between bg-white">
              <h2 className="text-lg font-bold text-[#142e3e]">Pratinjau template</h2>
              <Send className="h-5 w-5 fill-[#142e3e]" />
            </div>

            <div className="p-10 bg-[#e5ddd5] bg-[url('https://user-images.githubusercontent.com/15075759/28719144-86dc0f70-73b1-11e7-911d-60d70fcded21.png')] bg-repeat">
              <div className="bg-white rounded-xl shadow-md overflow-hidden relative max-w-[90%]">
                <div className="absolute top-0 left-[-8px] w-0 h-0 border-t-[10px] border-t-white border-l-[10px] border-l-transparent"></div>
                <div className="p-4 space-y-2">
                  {previewHeader && (
                    <h4 className="font-bold text-[#111b21] text-base leading-tight break-words" dangerouslySetInnerHTML={{ __html: previewHeader }} />
                  )}
                  <div className="text-[#111b21] text-sm leading-relaxed whitespace-pre-wrap break-words" dangerouslySetInnerHTML={{ __html: previewBody || '<span class="text-slate-300 italic">Body message goes here...</span>' }} />
                  <div className="flex items-end justify-between gap-4 mt-2">
                    {previewFooter ? <div className="text-[11px] text-[#667781] leading-tight flex-1" dangerouslySetInnerHTML={{ __html: previewFooter }} /> : <div className="flex-1" />}
                    <span className="text-[10px] text-[#667781] shrink-0">{new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                  </div>
                </div>
                {buttons.length > 0 && (
                  <div className="border-t border-[#f2f2f2] flex flex-col divide-y divide-[#f2f2f2]">
                    {buttons.map(btn => (
                      <div key={btn.id} className="py-2.5 px-3 text-[#00a8e6] font-medium text-center flex items-center justify-center gap-2 cursor-default">
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
